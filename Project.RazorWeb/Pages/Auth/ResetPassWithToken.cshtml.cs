using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Servie.Service.Auth;
using System.Threading.Tasks;

namespace Project.RazorWeb.Pages.Auth
{
    public class ResetPassWithTokenModel : PageModel
    {
        private readonly IAuthService _authService;

        public ResetPassWithTokenModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty]
        public string NewPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string Message { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (NewPassword != ConfirmPassword)
            {
                Message = "Passwords do not match.";
                return Page();
            }

            var result = await _authService.ResetPasswordAsync(Token, NewPassword);
            Message = result ? "Password has been reset successfully." : "Invalid or expired token.";

            return Page();
        }
    }
}
