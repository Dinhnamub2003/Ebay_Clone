using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Service.Service.Account;
using Project.Model.UserModel; // Sử dụng UserUpdateModel ở đây
using System.Threading.Tasks;
using Project.Data.Models;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;
using Project.EventRazor.Hubs;
using System.Security.Claims;

namespace Project.RazorWeb.Pages.UserProfile
{
    public class UpdateProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IHubContext<DocumentHub> _hubContext;

        public UpdateProfileModel(IUserService userService, IHubContext<DocumentHub> hubContext)
        {
            _userService = userService;
            _hubContext = hubContext;

        }

        [BindProperty]
        public UserUpdateModel UserUpdateModel { get; set; } // Sử dụng UserUpdateModel

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Chuyển dữ liệu từ User sang UserUpdateModel
            UserUpdateModel = new UserUpdateModel
            {
                UserId = user.UserId,
                Fullname = user.Fullname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address
            };

            return Page();
        }

        private User ConvertToUser(UserUpdateModel updateModel)
        {
            return new User
            {
                UserId = updateModel.UserId,
                Fullname = updateModel.Fullname,
                Email = updateModel.Email,
                PhoneNumber = updateModel.PhoneNumber,
                Address = updateModel.Address
            };
        }


        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || int.Parse(userIdClaim) != UserUpdateModel.UserId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var updatedUser = ConvertToUser(UserUpdateModel);
            var result = await _userService.UpdateUserProfileAsync(updatedUser);

            if (result)
            {
                // Lấy lại thông tin từ cơ sở dữ liệu
                var refreshedUser = await _userService.GetUserByIdAsync(updatedUser.UserId);

                // Kiểm tra log toàn bộ dữ liệu của refreshedUser
                Console.WriteLine($"[Debug] Fullname: {refreshedUser.Fullname}");
                Console.WriteLine($"[Debug] Email: {refreshedUser.Email}");
                Console.WriteLine($"[Debug] PhoneNumber: {refreshedUser.PhoneNumber}");
                Console.WriteLine($"[Debug] Address: {refreshedUser.Address}");

                // Gửi dữ liệu qua SignalR
                await _hubContext.Clients.All.SendAsync("ReceiveUpdatedUserProfile", new
                {
                    UserId = refreshedUser.UserId,
                    Fullname = refreshedUser.Fullname,
                    Email = refreshedUser.Email,
                    PhoneNumber = refreshedUser.PhoneNumber,
                    Address = refreshedUser.Address
                });

                return RedirectToPage("/UserProfile/UserDetail", new { id = UserUpdateModel.UserId });
            }

            ViewData["ErrorMessage"] = "Cập nhật thất bại. Vui lòng thử lại.";
            return Page();
        }


    }
}
