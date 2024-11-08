using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Model.ProductModel;
using Project.Servie.Service.Products;

namespace Project.RazorWeb.Pages.Wallet
{
    public class testModel : PageModel
    {
        private readonly IProductService _productService;

        public testModel(IProductService productService)
        {
            _productService = productService;
        }

        public ProductDetailViewModel Product { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Product = await _productService.GetProductWithImagesAsync(id);
            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
