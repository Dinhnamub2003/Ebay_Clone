using Microsoft.EntityFrameworkCore;
using Project.Bussiness.Infrastructure;
using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Service.Service.Backgound
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddProfileViewNotificationAsync(int viewedUserId, int viewerId)
        {
            // Retrieve the viewer and viewed user information from the database
            var viewedUser = await _unitOfWork.UserRepository.GetByIdAsync(viewedUserId);
            var viewer = await _unitOfWork.UserRepository.GetByIdAsync(viewerId);

            if (viewedUser == null || viewer == null || viewedUserId == viewerId)
            {
                return false; // Do not add a notification if the information is invalid or the viewer is the same as the viewed user
            }

            // Create a new notification
            var notification = new Notification
            {
                UserId = viewedUserId,
                Title = "New profile view",
                Message = $"{viewer.Fullname} has viewed your profile.",
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            _unitOfWork.NotificationRepository.Add(notification);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }




        // Lấy danh sách thông báo theo userId
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _unitOfWork.NotificationRepository.GetQuery()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAllNotificationsAsReadAsync(int userId)
        {
            var notifications = await _unitOfWork.NotificationRepository.GetQuery()
                .Where(n => n.UserId == userId && (n.IsRead == false || n.IsRead == null))
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _unitOfWork.SaveChangesAsync();
        }


        // Lấy số lượng thông báo chưa đọc
        public async Task<int> GetUnreadNotificationCountAsync(int userId)
        {
            return await _unitOfWork.NotificationRepository.GetQuery()
                .Where(n => n.UserId == userId && (n.IsRead == false || n.IsRead == null))
                .CountAsync();
        }



        // Cập nhật trạng thái đã đọc cho một thông báo
        public async Task<bool> MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = await _unitOfWork.NotificationRepository.GetByIdAsync(notificationId);
            if (notification == null || notification.IsRead == true)
            {
                return false; // Không tồn tại hoặc đã đọc
            }

            notification.IsRead = true;
            _unitOfWork.NotificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }















    }
}
