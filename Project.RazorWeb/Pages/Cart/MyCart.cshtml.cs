﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data.Models;
using Project.Model.CartModel;
using Project.Service.Service.Carts;
using Project.Service.Service.Order;
using Project.Service.Service.Orders;
using Project.Servie.Service.Products;
using System.Security.Claims;

namespace Project.RazorWeb.Pages.Cart
{
    public class CartModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartModel(ICartService cartService, IProductService productService, IOrderService orderService )
        {
            _productService = productService;
            _cartService = cartService;
            _orderService = orderService;
        }

        public List<CartViewModel> Carts { get; set; }
		[BindProperty]
		public ViewCreateCartModel CartsAdd { get; set; }

		public int AvailableQuantity { get; set; }
		public decimal TotalPrice { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {

            // Retrieve the user ID from the JWT claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // If there's no user ID claim, redirect to the login page
                return RedirectToPage("/auth/login");
            }

            var userId = int.Parse(userIdClaim);


            // Fetch the user's cart and other details
            Carts = await _cartService.GetCartByUserIdAsync(userId);
            TotalPrice = (decimal)Carts.Sum(c => c.Price * c.Quantity);
            AvailableQuantity = Carts.FirstOrDefault()?.AvailableQuantity ?? 0;

            return Page();
        }


        public async Task<IActionResult> OnPostDeleteAsync(int cartId)
		{
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // If there's no user ID claim, redirect to the login page
                return RedirectToPage("/auth/login");
            }

            var userId = int.Parse(userIdClaim);
            var result = await _cartService.DeleteCartAsync(cartId, userId); // Giả định bạn có `CurrentUserId`
			if (!result)
			{
				ModelState.AddModelError(string.Empty, "Failed to delete the item from the cart.");
			}
			return RedirectToPage(); 
		}

		public async Task<IActionResult> OnPostUpdateQuantityAsync(int cartId, int quantity)
		{
			// Lấy cart item theo CartId và cập nhật quantity
			var cartItem = await _cartService.GetCartItemByIdAsync(cartId);
			if (cartItem == null || cartItem.IsDeleted == true)
			{
				return NotFound();
			}

			// Cập nhật số lượng và lưu lại
			cartItem.Quantity = quantity;
			await _cartService.UpdateCartAsync(cartItem);

			// Reload lại trang để hiển thị số lượng mới
			return RedirectToPage();
		}


		public async Task<IActionResult> OnPostClearCartAsync()
		{
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // If there's no user ID claim, redirect to the login page
                return RedirectToPage("/auth/login");
            }

            var userId = int.Parse(userIdClaim);
            var result = await _cartService.ClearCartAsync(userId);

			if (!result)
			{
				ModelState.AddModelError(string.Empty, "Failed to clear the cart.");
			}

			// Tải lại danh sách sản phẩm trong giỏ hàng sau khi xóa
			Carts = await _cartService.GetCartByUserIdAsync(userId);
			TotalPrice = (decimal)Carts.Sum(c => c.Price * c.Quantity);

			return RedirectToPage();
		}

        public async Task<IActionResult> OnPostConfirmOrderAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // If there's no user ID claim, redirect to the login page
                return RedirectToPage("/auth/login");
            }

            var userId = int.Parse(userIdClaim);

            // Fetch the user's cart
            Carts = await _cartService.GetCartByUserIdAsync(userId);
            decimal totalAmount = 0;

            // Create the Order and retrieve the order ID
            var orderId = await _orderService.CreateOrderAsync(userId, totalAmount, "Confirmed");

            foreach (var cartItem in Carts)
            {
                var product = await _productService.GetProductWithImagesAsync((int)cartItem.ProductId);
                if (product != null)
                {
                    // Ensure product stock is sufficient for the quantity
                    if (product.Quantity >= cartItem.Quantity)
                    {
                        // Calculate the total amount for the order (sum of cart items)
                        totalAmount += cartItem.Price * (int)cartItem.Quantity;

                        // Create OrderDetail for each cart item with the correct quantity
                        var orderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = (int)cartItem.Quantity,  // Ensure the correct quantity is used
                        Price = cartItem.Price,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                        await _orderService.AddOrderDetailsAsync(orderId, orderDetails);
                        await _productService.UpdateProductQuantityAsync(product.ProductId, (int)cartItem.Quantity);
                    }
                    else
                    {
                        return RedirectToPage("/Error", new { message = $"Insufficient stock for {cartItem.ProductName}. Only {product.Quantity} available." });
                    }
                }
                else
                {
                    return RedirectToPage("/Error", new { message = $"Product not found for {cartItem.ProductName}" });
                }
            }

            // Update the order total price (it was updated in the loop)
          await _orderService.UpdateOrderTotalAmountAsync(orderId, totalAmount);

            // Clear the cart after the order is placed
            await _cartService.ClearCartAsync(userId);

            // Redirect to an order confirmation page
            return RedirectToPage("/Cart/checkout");
        }


    }
}