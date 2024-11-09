using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Model.CategoryModel;
using Project.Model.ProductModel;
using Project.Servie.Service.Categories;
using Project.Servie.Service.Products;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
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
                // Lấy ID người dùng từ claim `NameIdentifier`
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    //TempData["ErrorMessage"] = "User is not authenticated or token is missing required information.\n" ;
                    //Categories = await _categoryService.GetAllCategoriesAsync();
                    //return Page();

                    return RedirectToPage("/Auth/Login");
                }

                var userId = int.Parse(userIdClaim);
                var productId = await _productService.AddProductAsync(Product, userId);

                if (Product.Images != null && Product.Images.Count > 0)
                {
                    var savedImageUrls = await _productService.ValidateAndSaveImagesAsync(productId, Product.Images);
                    foreach (var url in savedImageUrls)
                    {
                        await _productService.SaveProductImageAsync(productId, url);
                    }
                }

                return RedirectToPage("/user/profile/", new { id = userId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                Categories = await _categoryService.GetAllCategoriesAsync();
                return Page();
            }
        }



    }
}
