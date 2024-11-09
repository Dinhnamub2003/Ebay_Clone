using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Data.Models;
using Project.Model.CartModel;
using Project.Service.Service.Carts;
using Project.Service.Service.Orders;
using Project.Service.Service.Wallets;
using Project.Servie.Service.Products;
using System.Security.Claims;

namespace Project.RazorWeb.Pages.Cart
{
    public class CheckOutModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        private readonly IWalletService _walletService;

        public CheckOutModel(IProductService productService, ICartService cartService,  IWalletService walletService)
        {
            _productService = productService;
            _cartService = cartService;
            _walletService = walletService;
        }

        public List<CartViewModel> Carts { get; set; }
        public decimal Balance { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out var userId))
            {
             
                Balance = await _walletService.GetWalletBalanceAsync(userId);
              
            }
            return Page();
        }

        
    

    }
}
