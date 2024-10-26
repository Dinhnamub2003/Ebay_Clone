using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Project.Servie.Service.Auth;

namespace Project.RazorWeb.Pages.Auth
{
    public class EmailVerificationModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ILogger<EmailVerificationModel> _logger; // Thêm logger

        public EmailVerificationModel(IAuthService authService, ILogger<EmailVerificationModel> logger)
        {
            _authService = authService;
            _logger = logger; // Khởi tạo logger
        }

        public string Message { get; set; } // Thông báo cho người dùng

        public async Task<IActionResult> OnGetAsync(string token, string code)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(code))
            {
                ModelState.AddModelError(string.Empty, "Invalid verification link.");
                return Page();
            }

            _logger.LogInformation($"Token: {token}, Code: {code}");

            var result = await _authService.VerifyEmailAsync(token, code);

            if (result)
            {
                TempData["SuccessMessage"] = "Xác minh thành công";
                return RedirectToPage("/auth/login");
            }

            ModelState.AddModelError(string.Empty, "Email verification failed.");
            return Page();
        }

    }
}
