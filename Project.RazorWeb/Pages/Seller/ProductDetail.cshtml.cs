using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Model.ProductModel;
using Project.Servie.Service.Products;
using System.Threading.Tasks;

namespace Project.RazorWeb.Pages.Seller
{
    public class ProductDetailModel : PageModel
    {
        private readonly IProductService _productService;

        public ProductDetailModel(IProductService productService)
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
