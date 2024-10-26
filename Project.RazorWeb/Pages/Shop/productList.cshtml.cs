using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Model.CategoryModel;
using Project.Model.ProductModel;
using Project.Servie.Service.Categories;
using Project.Servie.Service.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.RazorWeb.Pages.Shop
{
    public class ProductListModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductListModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public List<ProductViewModel> ProductViewModel { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public async Task<IActionResult> OnGetAsync(int? categoryId, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                if (categoryId.HasValue)
                {
                    ProductViewModel = await _productService.SearchProductsWithCategory(searchTerm, categoryId.Value);
                }
                else
                {
                    ProductViewModel = await _productService.SearchProducts(searchTerm); // Search across all categories
                }
            }
            else if (categoryId.HasValue)
            {
                ProductViewModel = await _productService.GetAllProductsByCategoryAsync(categoryId.Value);
            }
            else
            {
                ProductViewModel = await _productService.GetAllProducts();
            }

            if (ProductViewModel == null || !ProductViewModel.Any())
            {
                return Page();
            }
            Categories = await _categoryService.GetAllCategoriesAsync();
            return Page();
        }

    }
}
