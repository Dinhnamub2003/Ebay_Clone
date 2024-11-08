using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project.Data.Models;
using Project.Bussiness.Infrastructure;

namespace Project.Service.Service.Account
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Lấy thông tin người dùng theo ID
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _unitOfWork.UserRepository.GetQuery().FirstOrDefaultAsync(u => u.UserId == userId);
        }

        // Cập nhật thông tin hồ sơ người dùng
        public async Task<bool> UpdateUserProfileAsync(User user)
        {
            var existingUser = await _unitOfWork.UserRepository.GetByIdAsync(user.UserId);
            if (existingUser == null)
                return false;

            existingUser.Fullname = user.Fullname;
            existingUser.Email = user.Email;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Address = user.Address;
            existingUser.IsActive = user.IsActive;
            existingUser.UpdatedAt = DateTime.Now;

            _unitOfWork.UserRepository.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Cập nhật ảnh đại diện của người dùng
        public async Task<bool> UpdateUserAvatarAsync(int userId, IFormFile avatar)
        {
            if (avatar == null || avatar.Length > 10 * 1024 * 1024) // Giới hạn kích thước ảnh là 10MB
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(avatar.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                return false;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Đảm bảo thư mục "wwwroot/avatar" tồn tại
            var avatarFolder = Path.Combine("wwwroot", "avatar");
            if (!Directory.Exists(avatarFolder))
            {
                Directory.CreateDirectory(avatarFolder);
            }

            // Tạo đường dẫn cho file ảnh
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(avatarFolder, fileName);

            // Lưu ảnh vào đường dẫn
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatar.CopyToAsync(stream);
            }

            // Cập nhật đường dẫn avatar trong cơ sở dữ liệu
            user.Avatar = $"/avatar/{fileName}";
            user.UpdatedAt = DateTime.Now;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        // Lấy sản phẩm của người dùng cùng với ảnh
        public async Task<List<Product>> GetUserProductsWithImagesAsync(int userId)
        {
            return await _unitOfWork.ProductRepository.GetQuery()
                .Include(p => p.ProductImages)
                .Where(p => p.UserId == userId && (p.IsDeleted == null || p.IsDeleted == false))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
