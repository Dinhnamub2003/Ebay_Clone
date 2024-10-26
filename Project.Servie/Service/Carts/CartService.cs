using Project.Bussiness.Infrastructure;
using Project.Model.CartModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Servie.Service.Carts
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<int> AddToCart(CartViewModel model)
        {

            var cart = new Data.Models.Cart
            {
                UserId = model.UserId,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
                CreatedAt = DateTime.Now
            };

            // Thêm vào Repository
            _unitOfWork.CartRepository.Add(cart);
            await _unitOfWork.SaveChangesAsync();

            // Trả về CartId vừa tạo
            return cart.CartId;
        }

        public Task<bool> DeleteCartAsync(int cartId)
        {
            throw new NotImplementedException();
        }

        public Task<List<CartViewModel>> GetCartByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateCartAsync(CartViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}
