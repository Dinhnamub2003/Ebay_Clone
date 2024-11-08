using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Project.Data.Models;

namespace Project.Service.Service.Account
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int userId);
        Task<bool> UpdateUserProfileAsync(User user);
        Task<bool> UpdateUserAvatarAsync(int userId, IFormFile avatar);


        Task<List<Product>> GetUserProductsWithImagesAsync(int userId);
    }
}
