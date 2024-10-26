using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Project.Bussiness.Infrastructure;
using Project.Service.Service.Email;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project.Servie.Service.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IEmailSender emailSender, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await _unitOfWork.UserRepository.GetQuery()
                .Include(u => u.Role) // Nạp `Role` liên kết với `User`
                .FirstOrDefaultAsync(u => u.Username == username);


            if (user == null || !VerifyPasswordHash(password, user.Password))
            {
                _logger.LogWarning("Invalid username or password.");
                return "Invalid username or password.";
            }

            if (user.IsVerification == false)
            {
                // Người dùng chưa xác minh, gửi lại mã xác minh qua email
                var verificationCode = GenerateVerificationCode();
                user.Code = verificationCode;

                // Cập nhật lại mã xác minh trong database
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.SaveChangesAsync();

                // Gửi lại email xác minh
                var token1 = GenerateJwtToken(user, TimeSpan.FromMinutes(3));
                var verificationLink = $"http://localhost:5081/auth/verify-email?token={token1}&code={verificationCode}";

                await _emailSender.SendEmailAsync(user.Email, "Account Verification",
                    $@"
            <p>Your account is not yet verified. Please verify your account using the code: <strong>{verificationCode}</strong></p>
            <p>Or click the link below to verify your account (valid for 3 minutes):</p>
            <a href='{verificationLink}'>Verify Account</a>");

                _logger.LogWarning("Account is not verified. Verification email sent.");
                return "Account is not verified. Please check your email to verify your account.";
            }

            // Nếu đã xác minh, tạo JWT token
            var token = GenerateJwtToken(user, TimeSpan.FromMinutes(1440)); // Token expires in 24 hours
            return token;
        }

        public async Task<bool> RegisterAsync(Project.Data.Models.User user, string password)
        {
            if (await _unitOfWork.UserRepository.GetQuery().AnyAsync(u => u.Email == user.Email || u.Username == user.Username))
            {
                _logger.LogWarning("Email or username already exists.");
                return false;
            }

            var role = await _unitOfWork.RoleRepository.GetQuery().FirstOrDefaultAsync(r => r.RoleId == 2);
            if (role == null)
            {
                _logger.LogWarning("Role with ID 2 not found.");
                return false; // Xử lý khi Role không tồn tại
            }

            // Hash password and create user
            user.Password = CreatePasswordHash(password);
            var verificationCode = GenerateVerificationCode();
            user.Code = verificationCode;
            user.IsVerification = false;
            user.Role = role;

            _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate JWT token for email verification (valid for 3 minutes)
            var token = GenerateJwtToken(user, TimeSpan.FromMinutes(3));


            // Construct verification link
            var verificationLink = $"http://localhost:5081/auth/verify-email?token={token}&code={verificationCode}";

            // Send email including both the code and the verification link
            await _emailSender.SendEmailAsync(user.Email, "Account Verification",
                $@"
    <p>Thank you for registering!</p>
    <p>Please verify your account using the verification code: <strong>{verificationCode}</strong></p>
    <p>Or click the link below to verify your account (valid for 3 minutes):</p>
    <a href='{verificationLink}'>Verify Account</a>
    ");


            _logger.LogInformation("User registered successfully. Verification email sent.");
            return true;
        }

        public async Task<bool> ConfirmAccountAsync(string email, string code)
        {
            var user = await _unitOfWork.UserRepository.GetQuery().FirstOrDefaultAsync(u => u.Email == email && u.Code == code);

            if (user == null)
            {
                _logger.LogWarning("Invalid email or verification code.");
                return false;
            }

            // Confirm account
            user.IsVerification = true;
            user.Code = null;
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("User account verified successfully.");
            return true;
        }

        // Method to verify the token (added to AuthService)
        public async Task<bool> VerifyEmailAsync(string token, string code)
        {
            _logger.LogInformation("Starting email verification...");

            // Lấy khóa bí mật từ cấu hình
            var secretKey = _configuration["JWT:Secret"];
            if (string.IsNullOrEmpty(secretKey))
            {
                _logger.LogError("JWT secret key is missing in the configuration.");
                throw new ArgumentNullException(nameof(secretKey), "JWT secret key is not configured.");
            }

            var handler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };

            try
            {
                _logger.LogInformation("Validating JWT token...");

                var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);

                var emailClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (emailClaim != null)
                {
                    _logger.LogInformation($"Email claim found: {emailClaim.Value}. Confirming account...");
                    return await ConfirmAccountAsync(emailClaim.Value, code);
                }

                _logger.LogWarning("Email claim not present in the token.");
                return false;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("JWT token has expired.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while validating JWT token.");
                return false;
            }
        }


        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _unitOfWork.UserRepository.GetQuery().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("User with specified email does not exist.");
                return false;
            }

            // Generate reset token (JWT valid for 5 minutes)
            var resetToken = GenerateJwtToken(user, TimeSpan.FromMinutes(10));

            // Send email with reset link
            var resetLink = $"http://localhost:5081/auth/reset-password?token={resetToken}";
            await _emailSender.SendEmailAsync(email, "Password Reset Request",
                $@"
    <p>You requested a password reset. Click the link below to reset your password. This link is valid for 5 minutes and can only be used once:</p>
    <a href='{resetLink}'>Reset Password</a>");

            _logger.LogInformation("Password reset email sent.");
            return true;
        }

   public async Task<bool> ResetPasswordAsync(string token, string newPassword)
{
    //var secretKey = _configuration["JWT:Secret"];
    var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsTheSecureKey1234567890xinchaocacban"))
            };


            try
            {
        var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);
        var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);

        if (userIdClaim == null)
        {
            _logger.LogWarning("Token does not contain user ID.");
            return false;
        }

        int userId = int.Parse(userIdClaim.Value);
        var user = await _unitOfWork.UserRepository.GetQuery().FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            _logger.LogWarning("User not found for password reset.");
            return false;
        }

        // Update password
        user.Password = CreatePasswordHash(newPassword);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Password reset successfully.");
        return true;
    }
    catch (SecurityTokenExpiredException)
    {
        _logger.LogWarning("Password reset token has expired.");
        return false;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error while validating reset token.");
        return false;
    }
}



        private string DecodeJwtToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            var secretKey = _configuration["JWT:Secret"];

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false, // Adjust according to your needs
                ValidateAudience = false, // Adjust according to your needs
                ValidateIssuerSigningKey = false, // Adjust according to your security setup
                ValidateLifetime = true, // Check token expiration
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) 
            };

            try
            {
                var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);

                var emailClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
                if (emailClaim != null)
                {
                    return emailClaim.Value;
                }

                throw new ArgumentException("Email claim not present in token");
            }
            catch (SecurityTokenException)
            {
                throw new ArgumentException("Invalid JWT token");
            }
        }


        private string CreatePasswordHash(string password)
        {
            using (var hmac = new HMACSHA512())
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            //using (var hmac = new HMACSHA512())
            //{
            //    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            //    return storedHash == Convert.ToBase64String(computedHash);
            //}

            return password.Equals(storedHash);
        }

        private string GenerateVerificationCode()
        {
            var rng = new Random();
            return rng.Next(100000, 999999).ToString(); // Generate a 6-digit random code.
        }

        private string GenerateJwtToken(Project.Data.Models.User user, TimeSpan tokenLifetime)
        {
            var secretKey = _configuration["JWT:Secret"];


            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
      new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(tokenLifetime),
                SigningCredentials = credentials,
                Issuer = _configuration["JWT:ValidIssuer"], // Đảm bảo đúng Issuer
                Audience = _configuration["JWT:ValidAudience"] // Đảm bảo đúng Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
