using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data.Models;
using Project.Model.CartModel;
using Project.Service.Service.Carts;
using Project.Service.Service.Orders;
using Project.Servie.Service.Products;
using System.Security.Claims;

namespace Project.RazorWeb.Pages.Cart
{
    public class CheckOutModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CheckOutModel(IProductService productService, ICartService cartService, IOrderService orderService)
        {
            _productService = productService;
            _cartService = cartService;
            _orderService = orderService;
        }

        public List<CartViewModel> Carts { get; set; }

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
            decimal totalAmount = (decimal)Carts.Sum(item => item.Price * item.Quantity);

            // Create the Order and retrieve the order ID
            var orderId = await _orderService.CreateOrderAsync(userId, totalAmount, "Confirmed");

            foreach (var cartItem in Carts)
            {
                var product = await _productService.GetProductWithImagesAsync((int)cartItem.ProductId);
                if (product != null)
                {
                    // Check if the product's quantity in stock is sufficient
                    if (product.Quantity >= cartItem.Quantity)
                    {
                        // Create OrderDetail for each cart item
                        var orderDetails = new List<OrderDetail>
                {
                    new OrderDetail
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = (int)cartItem.Quantity,
                        Price = (decimal)cartItem.Price,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                        await _orderService.AddOrderDetailsAsync(orderId, orderDetails);
                        // Update product quantity in stock
                        await _productService.UpdateProductQuantityAsync(product.ProductId, (int)cartItem.Quantity);

                        // If stock is insufficient, return error (optional to handle differently)
                    }
                    else
                    {
                        // Handle case where there is insufficient stock
                        return RedirectToPage("/Error", new { message = $"Insufficient stock for {cartItem.ProductName}. Only {product.Quantity} available." });
                    }
                }
                else
                {
                    // Handle case where the product was not found (optional)
                    return RedirectToPage("/Error", new { message = $"Product not found for {cartItem.ProductName}" });
                }
            }

            // Clear the cart after order is placed
            await _cartService.ClearCartAsync(userId);

            // Redirect to an order confirmation page
            return RedirectToPage();
        }

    }
}
