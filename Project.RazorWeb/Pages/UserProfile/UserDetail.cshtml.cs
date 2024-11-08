using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data.Models;
using Project.Service.Service.Account;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Project.RazorWeb.Pages.UserProfile
{
    //[Authorize]
    public class UserDetailModel : PageModel
    {
        private readonly IUserService _userService;

        public UserDetailModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User Users { get; set; }

        public List<Product> UserProducts { get; set; } = new List<Product>();
        public async Task<IActionResult> OnGetAsync(int id)
        {
            //var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userIdClaim))
            //{
            //    TempData["ErrorMessage"] = "User is not authenticated or token is missing required information.";
            //    return RedirectToPage("/Auth/Login");
            //}

            //int userId = int.Parse(userIdClaim);
            Users = await _userService.GetUserByIdAsync(id);
            if (Users == null)
            {
                return NotFound();
            }
            UserProducts = await _userService.GetUserProductsWithImagesAsync(id);
            return Page();
        }

        public async Task<IActionResult> OnPostUploadAvatarAsync(IFormFile avatar)
        {
            if (avatar != null && avatar.Length < 10 * 1024 * 1024) // Kiểm tra kích thước <10 MB
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = System.IO.Path.GetExtension(avatar.FileName).ToLower();

                if (allowedExtensions.Contains(extension))
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(userIdClaim))
                    {
                        TempData["ErrorMessage"] = "User is not authenticated or token is missing required information.";
                        return RedirectToPage("/Auth/Login");
                    }

                    int userId = int.Parse(userIdClaim);

                    var result = await _userService.UpdateUserAvatarAsync(userId, avatar);
                    if (result)
                    {
                        return RedirectToPage();
                    }
                    ModelState.AddModelError(string.Empty, "Cập nhật avatar thất bại.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG và PNG.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Dung lượng ảnh vượt quá 10MB. Vui lòng chọn ảnh nhỏ hơn.");
            }
            return Page();
        }
    }
}
