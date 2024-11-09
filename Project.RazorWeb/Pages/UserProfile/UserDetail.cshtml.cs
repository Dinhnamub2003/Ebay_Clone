using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data.Models;
using Project.Service.Service.Account;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project.EventRazor.Hubs;
using Project.Service.Service.Backgound;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Project.RazorWeb.Pages.UserProfile
{
    [Authorize]
    public class UserDetailModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IHubContext<DocumentHub> _hubContext;
        private readonly INotificationService _notificationService;

        public UserDetailModel(IUserService userService, IHubContext<DocumentHub> hubContext, INotificationService notificationService)
        {
            _userService = userService;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        [BindProperty]
        public User Users { get; set; }

        public List<Product> UserProducts { get; set; } = new List<Product>();







        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                TempData["ErrorMessage"] = "User is not authenticated or token is missing required information.";
                return RedirectToPage("/Auth/Login");
            }


            int viewerId = int.Parse(userIdClaim);
            if (id != viewerId)
            {
                await _notificationService.AddProfileViewNotificationAsync(id, viewerId);
                var userClient = await _userService.GetUserByIdAsync(viewerId);

                await _hubContext.Clients.User(id.ToString()).SendAsync("ReceiveNotification", new
                {
                    title = "New profile view",
                    message = $"{userClient.Fullname} has viewed your profile.",
                    createdAt = DateTime.Now
                });

            }


            Users = await _userService.GetUserByIdAsync(id);
            if (Users == null)
            {
                return NotFound();
            }


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
