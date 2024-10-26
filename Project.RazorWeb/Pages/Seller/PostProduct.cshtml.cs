using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Model.CategoryModel;
using Project.Model.ProductModel;
using Project.Servie.Service.Categories;
using Project.Servie.Service.Products;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Project.RazorWeb.Pages.Seller
{
    public class PostProductModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public PostProductModel(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public ViewCreateProductModel Product { get; set; }

        public List<CategoryViewModel> Categories { get; set; }

        public async Task OnGetAsync()
        {
            Categories = await _categoryService.GetAllCategoriesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _categoryService.GetAllCategoriesAsync();
                return Page();
            }

            try
            {
                var productId = await _productService.AddProductAsync(Product);

                if (Product.Images != null && Product.Images.Count > 0)
                {
                    var savedImageUrls = await _productService.ValidateAndSaveImagesAsync(productId, Product.Images);
                    foreach (var url in savedImageUrls)
                    {
                        await _productService.SaveProductImageAsync(productId, url);
                    }
                }

                return RedirectToPage("/Seller/ProductDetail", new { id = productId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Categories = await _categoryService.GetAllCategoriesAsync();
                return Page();
            }
        }

    }
}
