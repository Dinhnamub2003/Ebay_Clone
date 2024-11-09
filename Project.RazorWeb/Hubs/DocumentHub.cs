using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Project.EventRazor.Hubs
{
    public class DocumentHub : Hub
    {
        public async Task UpdateUserProfile(int userId, string fullname, string email, string phoneNumber, string address)
        {
            // Gửi dữ liệu cập nhật tới tất cả các client đang kết nối
            await Clients.All.SendAsync("ReceiveUpdatedUserProfile", new
            {
                UserId = userId,
                Fullname = fullname,
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address
            });
        }


        public async Task SendProfileViewNotification(int userId, string viewerFullname)
        {
            var message = new
            {
                title = "New profile view",
                message = $"{viewerFullname} has viewed your profile.",
                createdAt = DateTime.Now
            };

            await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }



        public async Task SendNotificationToUser(int userId, string message)
        {
            await Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }

        public async Task UpdateProductDetail(int productId, string productName, string description, int quantity, decimal price, string brandName, string categoryName, List<string> imageUrls)
        {
            await Clients.All.SendAsync("ReceiveUpdatedProductDetail", new
            {
                ProductId = productId,
                ProductName = productName,
                Description = description,
                Quantity = quantity,
                Price = price,
                BrandName = brandName,
                CategoryName = categoryName, // Thêm danh mục vào dữ liệu gửi đi
                ImageUrls = imageUrls
            });
        }

    }
}
