using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Service.Service.Captcha;
using Project.Servie.Service.Auth;

namespace Project.RazorWeb.Pages.Auth
{
    public class loginModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ICaptchaService _captchaService;

        public loginModel(IAuthService authService, ICaptchaService captchaService)
        {
            _authService = authService;
            _captchaService = captchaService;
        }

        [BindProperty]
        public string Username { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string CaptchaInput { get; set; }

        public string ErrorMessage { get; set; }
        public string GeneratedCaptcha { get; private set; }

        public void OnGet()
        {
            // Generate CAPTCHA code on page load
            GeneratedCaptcha = _captchaService.GenerateCaptchaCode(6);
            HttpContext.Session.SetString("CaptchaCode", GeneratedCaptcha);
            if (TempData["SuccessMessage"] != null)
            {
                ErrorMessage = TempData["SuccessMessage"].ToString(); // Hiển thị thông báo
            }
        }



        public async Task<IActionResult> OnPostAsync()
        {

            //var actualCaptcha = HttpContext.Session.GetString("CaptchaCode");
            //if (string.IsNullOrEmpty(actualCaptcha) || !_captchaService.ValidateCaptchaCode(CaptchaInput, actualCaptcha))
            //{
            //    ErrorMessage = "Invalid CAPTCHA.";
            //    return Page();
            //}

            //if (ModelState.IsValid)
            //{
                var result = await _authService.LoginAsync(Username, Password);

                if (result == "Account is not verified. Please check your email to verify your account.")
                {
                    TempData["InfoMessage"] = result; // Lưu thông báo vào TempData để hiển thị trên trang xác minh
                    return RedirectToPage("/auth/verifyEmailCode"); // Chuyển hướng đến trang nhập mã xác minh
                }

                if (result == "Invalid username or password.")
                {
                    ModelState.AddModelError(string.Empty, result);
                    return Page();
                }

                // Nếu đăng nhập thành công, chuyển hướng đến trang chính
                return RedirectToPage("/index");
            //}

            //return Page();
        }


        //public async Task<IActionResult> OnPostLoginAsync()
        //{
        //    var actualCaptcha = HttpContext.Session.GetString("CaptchaCode");
        //    if (string.IsNullOrEmpty(actualCaptcha) || !_captchaService.ValidateCaptchaCode(CaptchaInput, actualCaptcha))
        //    {
        //        ErrorMessage = "Invalid CAPTCHA.";
        //        return Page();
        //    }

        //    var result = await _authService.LoginAsync(Username, Password);
        //    if (result.StartsWith("Invalid") || result.Contains("not verified"))
        //    {
        //        ErrorMessage = result;
        //        return Page();
        //    }

        //    return RedirectToPage("/Index");
        //}

        public IActionResult OnGetCaptcha()
        {
            var captchaCode = _captchaService.GenerateCaptchaCode(6);
            HttpContext.Session.SetString("CaptchaCode", captchaCode);

            var captchaImage = _captchaService.GenerateCaptchaImage(captchaCode);
            return File(captchaImage, "image/png");
        }
    }
}
