using Microsoft.AspNetCore.Http;
using Project.Model.ProductModel;
using System.Threading.Tasks;

namespace Project.Servie.Service.Products
{
    public interface IProductService
    {
        Task<int> AddProductAsync(ViewCreateProductModel model, int userId);
        Task SaveProductImageAsync(int productId, string imageUrl);
        Task<ProductDetailViewModel> GetProductWithImagesAsync(int productId);
        Task<List<string>> ValidateAndSaveImagesAsync(int productId, List<IFormFile> images);
        Task<bool> UpdateProductAsync(int productId, ViewCreateProductModel model, List<IFormFile> newImages, List<string> imagesToKeep);
        Task<List<ProductViewModel>> GetAllProductsByCategoryAsync(int categoryId);
        Task<List<ProductViewModel>> GetAllProducts();
        Task<List<ProductViewModel>> SearchProducts(string name);
        Task<List<ProductViewModel>> SearchProductsWithCategory(string name, int categoryId);
        Task<decimal> GetProductPriceById(int productId);
        Task UpdateProductQuantityAsync(int productId, int quantity);
    }
}
