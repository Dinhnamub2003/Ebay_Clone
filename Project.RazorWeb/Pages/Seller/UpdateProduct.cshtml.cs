using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Project.EventRazor.Hubs;
using Project.Model.CategoryModel;
using Project.Model.ProductModel;
using Project.Servie.Service.Categories;
using Project.Servie.Service.Products;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.RazorWeb.Pages.Seller
{
    [Authorize(Roles = "Admin")]
    public class UpdateProductModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IHubContext<DocumentHub> _hubContext;

        public UpdateProductModel(IProductService productService, ICategoryService categoryService, IHubContext<DocumentHub> hubContext)
        {
            _productService = productService;
            _categoryService = categoryService;
            _hubContext = hubContext;

        }

        [BindProperty]
        public ViewCreateProductModel Product { get; set; } = new ViewCreateProductModel();

        public List<CategoryViewModel> Categories { get; set; }
        public List<string> ExistingImageUrls { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var product = await _productService.GetProductWithImagesAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Set the properties of ViewCreateProductModel with the existing product details
            Product = new ViewCreateProductModel
            {
                ProductName = product.ProductName,
                Description = product.Description,
                Quantity = product.Quantity,
                BrandName = product.BrandName,
                Price = product.Price ?? 0,
                CategoryId = product.CategoryId
            };

            ExistingImageUrls = product.ImageUrls;
            Categories = await _categoryService.GetAllCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                Categories = await _categoryService.GetAllCategoriesAsync();
                return Page();
            }

            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return RedirectToPage("/Auth/Login");
                }

                var userId = int.Parse(userIdClaim);
                var imagesToKeep = Request.Form["ExistingImageUrls"].ToList();
                var updated = await _productService.UpdateProductAsync(id, Product, Product.Images, imagesToKeep);

                if (updated)
                {
                    var updatedProduct = await _productService.GetProductWithImagesAsync(id);

                    // Gửi thông tin cập nhật bao gồm CategoryName đến client qua SignalR
                    await _hubContext.Clients.All.SendAsync("ReceiveUpdatedProductDetail", new
                    {
                        ProductId = updatedProduct.ProductId,
                        ProductName = updatedProduct.ProductName,
                        Description = updatedProduct.Description,
                        Quantity = updatedProduct.Quantity,
                        Price = updatedProduct.Price,
                        BrandName = updatedProduct.BrandName,
                        CategoryName = updatedProduct.CategoryName, // Thêm CategoryName
                        ImageUrls = updatedProduct.ImageUrls
                    });

                    TempData["SuccessMessage"] = "Product updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Product not found or update failed.";
                }

                return RedirectToPage("/Seller/ProductDetail", new { id });
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
