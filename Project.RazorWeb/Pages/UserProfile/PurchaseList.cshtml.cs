using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Model.OrderModel;
using Project.Service.Service.Carts;
using Project.Service.Service.Orders;
using Project.Servie.Service.Products;
using System.Security.Claims;

namespace Project.RazorWeb.Pages.UserProfile
{
    public class PurchaseListModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public PurchaseListModel(IProductService productService, IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;
        }
        public List<ViewOrderModel> OrderList { get;set; }  
        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // If there's no user ID claim, redirect to the login page
                return RedirectToPage("/auth/login");
            }

            var userId = int.Parse(userIdClaim);
            OrderList = await _orderService.GetOrderByUserIdAsync(userId);
            

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmAsync(int orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // If there's no user ID claim, redirect to the login page
                return RedirectToPage("/auth/login");
            }
            await _orderService.UpdateOrderStatusAsync(orderId, "Confirmed");


            return RedirectToPage();
        }

    }
}

