using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Servie.Service.Auth;

namespace Project.RazorWeb.Pages.Auth
{
    public class EmailVerificationCodeModel : PageModel
    {
        private readonly IAuthService _authService;

        public EmailVerificationCodeModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string VerificationCode { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.ConfirmAccountAsync(Email, VerificationCode);
                if (result)
                {
                    TempData["SuccessMessage"] = "Xác minh thành công";
                    return RedirectToPage("/auth/login"); // Chuyển hướng đến trang đăng nhập sau khi xác minh thành công
                }
                ModelState.AddModelError(string.Empty, "Email hoặc mã xác minh không hợp lệ.");
            }

            return Page();
        }
    }
}




