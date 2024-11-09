using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data.Models;
using Project.Model.CategoryModel;
using Project.Service.Service.Backgound;
using Project.Service.Service.Wallets;
using Project.Servie.Service.Categories;
using System.Security.Claims;

namespace Project.RazorWeb.Pages.Auth.Home
{
    public class homeModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IWalletService _walletService;
        private readonly INotificationService _notificationService;

        public homeModel(ICategoryService categoryService, IWalletService walletService, INotificationService notificationService)
        {
            _categoryService = categoryService;
            _walletService = walletService;
            _notificationService = notificationService;
        }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public decimal Balance { get; set; }

        public int UnreadNotificationCount { get; set; }
        public List<CategoryViewModel> Categories { get; set; }

        public List<Notification> Notifications { get; set; } = new List<Notification>();





        public async Task<IActionResult> OnPostMarkAllNotificationsAsReadAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized();
            }

            int userId = int.Parse(userIdClaim);

            // Đánh d?u t?t c? thông báo là đ? đ?c
            await _notificationService.MarkAllNotificationsAsReadAsync(userId);

            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Lấy userIdClaim và phân tích nếu người dùng đã đăng nhập
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdClaim, out var userId))
                {
                    // Lấy thông tin số dư và tên người dùng
                    Balance = await _walletService.GetWalletBalanceAsync(userId);
                    UserName = User.Identity.Name;
                    UserId = userId;





                    UnreadNotificationCount = await _notificationService.GetUnreadNotificationCountAsync(userId);

                 
                    Notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);


                }
                else
                {
                    // Xử lý nếu không thể phân tích userId
                    return RedirectToPage("/Error", new { message = "User ID is invalid." });
                }






            }

            // Lấy danh sách danh mục cho cả người dùng đăng nhập và chưa đăng nhập
            Categories = await _categoryService.GetAllCategoriesAsync();

            return Page();
        }

    }
}
