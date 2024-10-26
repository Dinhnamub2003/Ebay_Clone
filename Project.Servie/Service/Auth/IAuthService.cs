using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Servie.Service.Auth
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(Project.Data.Models.User user, string password);
        Task<bool> ConfirmAccountAsync(string email, string code);
        Task<bool> VerifyEmailAsync(string token, string code);

        Task<bool> ForgotPasswordAsync(string email);

        Task<bool> ResetPasswordAsync(string token, string newPassword);
    }
}
