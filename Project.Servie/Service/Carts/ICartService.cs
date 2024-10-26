using Project.Model.CartModel;
using Project.Model.CategoryModel;
using Project.Model.ProductModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Servie.Service.Carts
{
    public interface ICartService
    {
        Task<int> AddToCart(CartViewModel model);
        Task<bool> UpdateCartAsync(CartViewModel model);
        Task<bool> DeleteCartAsync(int cartId);
        Task<List<CartViewModel>> GetCartByUserIdAsync(int userId);
    }
}
