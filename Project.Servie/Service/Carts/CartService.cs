using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Project.Bussiness.Infrastructure;
using Project.Data.Models;
using Project.Model.CartModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Service.Carts
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }
        public async Task<int> AddToCart(ViewCreateCartModel model)
        {

            var cart = new Data.Models.Cart
            {
                UserId = model.UserId,
                ProductId = model.ProductId,
                Quantity = model.Quantity,
				
                CreatedAt = DateTime.Now
            };

            
            _unitOfWork.CartRepository.Add(cart);
            await _unitOfWork.SaveChangesAsync();

          
            return cart.CartId;
        }

		public async Task<bool> DeleteCartAsync(int cartId, int userId)
		{
			try
			{
				// Lấy sản phẩm trong giỏ hàng theo cartId và userId
				var cartItem = await _unitOfWork.CartRepository.GetQuery()
					.FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId && c.IsDeleted == false);

				if (cartItem == null)
				{
					return false; // Không tìm thấy sản phẩm để xóa
				}

				// Đánh dấu sản phẩm là đã xóa
				cartItem.IsDeleted = true;
				cartItem.DeletedAt = DateTime.Now;

				// Cập nhật sản phẩm
				_unitOfWork.CartRepository.Update(cartItem);
				await _unitOfWork.SaveChangesAsync();

				return true;
			}
			catch (Exception ex)
			{
				// Ghi lại lỗi (thay thế với logger thực tế nếu có)
				Console.WriteLine($"Error deleting cart item: {ex.Message}");
				return false;
			}
		}


		public async Task<List<CartViewModel>> GetCartByUserIdAsync(int userId)
        {
            var cart = await _unitOfWork.CartRepository
                .GetQuery()
                .Where(c => c.UserId == userId && c.IsDeleted == false && c.DeletedAt == null)
                .Include(c => c.Product) // Tải dữ liệu sản phẩm
                .ThenInclude(p => p.ProductImages) // Tải dữ liệu hình ảnh sản phẩm
                .Where(c => c.Product != null && c.Product.DeletedAt == null) // Kiểm tra sản phẩm chưa bị xóa
                .ToListAsync();

            return cart.Select(c => new CartViewModel
            {
				CartId = c.CartId,
				ProductId = c.ProductId ?? 0,
                UserId = c.UserId ?? 0,
                Quantity = c.Quantity ?? 1,
                ProductName = c.Product?.ProductName ?? "Unknown",
                Description = c.Product?.Description ?? "No description",
                Brand = c.Product?.BrandName ?? "No brand",
                Price = (decimal)c.Product.Price,
				AvailableQuantity = c.Product?.Quantity ?? 0,
                ImageUrls = c.Product?.ProductImages.Select(i => i.ImageUrl).ToList()
            }).ToList();
        }


		public async Task<bool> UpdateCartAsync(Cart model)
        {
            var cart = await _unitOfWork.CartRepository.GetByIdAsync(model.CartId);
            if (cart == null || cart.IsDeleted == true)
            {
                return false;

            }
            cart.Quantity = model.Quantity; 
            cart.UpdatedAt = DateTime.Now;
            _unitOfWork.CartRepository.Update(cart);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

		public async Task<bool> ClearCartAsync(int userId)
		{
			try
			{
				// Lấy danh sách các sản phẩm trong giỏ hàng của người dùng chưa bị xóa
				var cartItems = await _unitOfWork.CartRepository
					.GetQuery()
					.Where(c => c.UserId == userId && c.IsDeleted == false)
					.ToListAsync();

				if (!cartItems.Any())
				{
					return false; // Không có sản phẩm trong giỏ hàng
				}

				// Đánh dấu tất cả sản phẩm là đã xóa
				foreach (var item in cartItems)
				{
					item.IsDeleted = true;
					item.DeletedAt = DateTime.Now;
				}

				// Cập nhật thay đổi
				_unitOfWork.CartRepository.Update(cartItems);
				await _unitOfWork.SaveChangesAsync();

				return true;
			}
			catch (Exception ex)
			{
				// Log lỗi nếu cần
				Console.WriteLine($"Error clearing cart: {ex.Message}");
				return false;
			}
		}

		public async Task<Cart> GetCartItemByIdAsync(int cartId)
		{
			return await _unitOfWork.CartRepository
	   .GetQuery()
	   .FirstOrDefaultAsync(c => c.CartId == cartId && c.IsDeleted == false);
		}

		public async Task<Cart> GetCartItemByProductIdAsync(int userId, int productId)
		{
			return await _unitOfWork.CartRepository
				.GetQuery() 
				.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId && c.IsDeleted == false);
		}



	}
}
