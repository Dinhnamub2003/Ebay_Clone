
using Project.Data.Models;
using Project.Model.CartModel;

namespace Project.Service.Service.Carts
{
    public interface ICartService
    {
        Task<int> AddToCart(ViewCreateCartModel model);
        Task<bool> UpdateCartAsync(Cart model);
        Task<bool> DeleteCartAsync(int cartId, int userId);
        Task<List<CartViewModel>> GetCartByUserIdAsync(int userId);
        Task<bool> ClearCartAsync(int userId);

		Task<Cart> GetCartItemByIdAsync(int cartId);

		Task<Cart> GetCartItemByProductIdAsync(int cartId, int productId);


	}
}
