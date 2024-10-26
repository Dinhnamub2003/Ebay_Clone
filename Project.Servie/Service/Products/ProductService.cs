using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project.Bussiness.Infrastructure;
using Project.Data.Models;
using Project.Model.ProductModel;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Servie.Service.Products
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> AddProductAsync(ViewCreateProductModel model)
        {
            var product = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Quantity = model.Quantity,
                BrandName = model.BrandName,
                CreatedAt = DateTime.Now,
                CategoryId = model.CategoryId // Gán CategoryId từ model
            };

            _unitOfWork.ProductRepository.Add(product);
            await _unitOfWork.SaveChangesAsync();

            return product.ProductId;
        }

        public async Task SaveProductImageAsync(int productId, string imageUrl)
        {
            var productImage = new ProductImage
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.Now
            };

            _unitOfWork.ProductImageRepository.Add(productImage);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ProductDetailViewModel> GetProductWithImagesAsync(int productId)
        {
            var product = await _unitOfWork.ProductRepository.GetQuery()
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductDetailViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    BrandName = p.BrandName,
                    CreatedAt = p.CreatedAt,
                    ImageUrls = p.ProductImages.Select(i => i.ImageUrl).ToList()
                })
                .FirstOrDefaultAsync();

            return product;
        }


        public async Task<List<string>> ValidateAndSaveImagesAsync(int productId, List<IFormFile> images)
        {
            const int maxImageCount = 10;
            const long maxImageSize = 10 * 1024 * 1024; // 10 MB
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var savedImageUrls = new List<string>();

            if (images.Count > maxImageCount)
                throw new ArgumentException("Cannot upload more than 10 images.");

            foreach (var image in images)
            {
                if (image.Length > maxImageSize)
                    throw new ArgumentException("Image size cannot exceed 10MB.");

                var extension = Path.GetExtension(image.FileName).ToLower();
                if (!allowedExtensions.Contains(extension))
                    throw new ArgumentException("Invalid file type. Only .jpg, .jpeg, .png are allowed.");

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, image.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                savedImageUrls.Add($"/uploads/{image.FileName}");
            }

            return savedImageUrls;
        }

        public async Task<List<ProductViewModel>> GetAllProductsByCategoryAsync(int categoryId)
        {
            var products = await _unitOfWork.ProductRepository.GetQuery()
                .Where(p => p.CategoryId == categoryId)
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    BrandName = p.BrandName,
                    CreatedAt = p.CreatedAt,
                    ImageUrls = p.ProductImages.Select(i => i.ImageUrl).Take(1).ToList(), 
                    CategoryName = p.Category != null ? p.Category.CategoryName : "No Category"
                })
                .ToListAsync();

            return products;
        }

        

        public async Task<List<ProductViewModel>> GetAllProducts()
        {
            var products = await _unitOfWork.ProductRepository.GetQuery()
                    .Select(p => new ProductViewModel
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Description = p.Description,
                        Quantity = p.Quantity,
                        BrandName = p.BrandName,
                        CreatedAt = p.CreatedAt,
                        ImageUrls = p.ProductImages.Select(i => i.ImageUrl).Take(1).ToList(),
                        CategoryName = p.Category != null ? p.Category.CategoryName : "No Category"
                    })
                    .ToListAsync();

            return products;
        }

        public async Task<List<ProductViewModel>> SearchProducts(string name)
        {
            var products = await _unitOfWork.ProductRepository.GetQuery()
                .Where(p => p.ProductName.Contains(name)) 
                .Select(p => new ProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    BrandName = p.BrandName,
                    CreatedAt = p.CreatedAt,
                    ImageUrls = p.ProductImages.Select(i => i.ImageUrl).Take(1).ToList(),
                    CategoryName = p.Category != null ? p.Category.CategoryName : "No Category"
                })
                .ToListAsync();

            return products;
        }

        public async Task<List<ProductViewModel>> SearchProductsWithCategory(string name, int categoryId)
        {
            var products = await _unitOfWork.ProductRepository.GetQuery()
                 .Where(p => p.ProductName.Contains(name) && p.CategoryId == categoryId)
                    .Select(p => new ProductViewModel
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Description = p.Description,
                        Quantity = p.Quantity,
                        BrandName = p.BrandName,
                        CreatedAt = p.CreatedAt,
                        ImageUrls = p.ProductImages.Select(i => i.ImageUrl).Take(1).ToList(),
                        CategoryName = p.Category != null ? p.Category.CategoryName : "No Category"
                    })
                    .ToListAsync();

            return products;
        }
    }
}
