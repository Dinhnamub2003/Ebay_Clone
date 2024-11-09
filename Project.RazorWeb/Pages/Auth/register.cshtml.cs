using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data.Models;
using Project.Servie.Service.Auth;
using Project.Service.Service.Captcha;

namespace Project.RazorWeb.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IAuthService _authService;
        private readonly ICaptchaService _captchaService; // Dịch vụ CAPTCHA

        public RegisterModel(IAuthService authService, ICaptchaService captchaService)
        {
            _authService = authService;
            _captchaService = captchaService; // Khởi tạo dịch vụ CAPTCHA
        }

        [BindProperty]
        public User User { get; set; } = new User();

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        public string CaptchaInput { get; set; } // Giá trị CAPTCHA nhập từ người dùng

        public void OnGet()
        {
            // Tạo CAPTCHA khi trang được load
            var captchaCode = _captchaService.GenerateCaptchaCode(6);
            HttpContext.Session.SetString("CaptchaCode", captchaCode); // Lưu CAPTCHA vào session
        }

        public IActionResult OnGetCaptcha()
        {
            var captchaCode = HttpContext.Session.GetString("CaptchaCode");
            var captchaImage = _captchaService.GenerateCaptchaImage(captchaCode);

            return File(captchaImage, "image/png"); // Trả về CAPTCHA dưới dạng hình ảnh
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra CAPTCHA
            //var storedCaptcha = HttpContext.Session.GetString("CaptchaCode");
            //if (string.IsNullOrEmpty(storedCaptcha) || CaptchaInput != storedCaptcha)
            //{
            //    ModelState.AddModelError("CaptchaInput", "CAPTCHA không đúng.");
            //    return Page();
            //}

            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu không khớp.");
                return Page();
            }

            var result = await _authService.RegisterAsync(User, Password);

            if (result)
            {
                return RedirectToPage("/auth/login"); // Chuyển hướng đến trang xác minh email
            }

            ModelState.AddModelError(string.Empty, "Email hoặc tên đăng nhập đã tồn tại.");
            return Page();
        }
    }
}
