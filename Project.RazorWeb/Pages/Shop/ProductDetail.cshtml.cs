using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Model.CartModel;
using Project.Model.ProductModel;
using Project.Service.Service.Carts;
using Project.Servie.Service.Products;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.RazorWeb.Pages.Seller
{
    public class ProductDetailModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public ProductDetailModel(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public ProductDetailViewModel Product { get; set; }
        [BindProperty]
        public ViewCreateCartModel CartsAdd { get; set; }
        public int? CurrentUserId { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                CurrentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            }
            Product = await _productService.GetProductWithImagesAsync(id);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                // If there's no user ID claim, redirect to the login page
                return RedirectToPage("/auth/login");
            }

            var userId = int.Parse(userIdClaim);
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                CartsAdd.UserId = userId; // Assuming UserId is set here, or get it dynamically as needed

                // Check if the item already exists in the cart
                var existingCartItem = await _cartService.GetCartItemByProductIdAsync((int)CartsAdd.UserId, (int)CartsAdd.ProductId);

                if (existingCartItem != null)
                {
                    // If product already exists in the cart, simply redirect to the cart page without updating quantity
                    return RedirectToPage("/Cart/MyCart"); // Redirect to cart without modifying quantity
                }
                else
                {
                    // Add new item to the cart if it doesn't exist
                    await _cartService.AddToCart(CartsAdd);
                }

                // Redirect to the cart page after adding a new item
                return RedirectToPage("/Cart/MyCart");
            }
            catch (Exception ex)
            {
                // Log exception and handle errors
                ModelState.AddModelError(string.Empty, "An error occurred while adding the item to the cart.");
                return Page();
            }
        }


    }
}

