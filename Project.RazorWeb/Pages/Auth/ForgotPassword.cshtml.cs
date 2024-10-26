using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Servie.Service.Auth;
using System.Threading.Tasks;

namespace Project.RazorWeb.Pages.Auth
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly IAuthService _authService;

        public ForgotPasswordModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public string Email { get; set; }

        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                Message = "Please enter a valid email.";
                return Page();
            }

            var result = await _authService.ForgotPasswordAsync(Email);
            Message = result ? "Password reset link has been sent to your email." : "Email not found.";

            return Page();
        }
    }
}
