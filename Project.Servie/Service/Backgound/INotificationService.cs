
using System.Collections.Generic;
using System.Threading.Tasks;
using Project.Data.Models;

namespace Project.Service.Service.Backgound
{
    public interface INotificationService
    {
        Task<bool> AddProfileViewNotificationAsync(int viewedUserId, int viewerId);

        // Lấy danh sách thông báo theo userId
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);

        // Cập nhật trạng thái đã đọc cho thông báo
        Task<bool> MarkNotificationAsReadAsync(int notificationId);

        Task MarkAllNotificationsAsReadAsync(int userId);

        // Lấy số lượng thông báo chưa đọc
        Task<int> GetUnreadNotificationCountAsync(int userId);
    }
}
