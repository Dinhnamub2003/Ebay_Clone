using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
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

        public async Task<int> AddProductAsync(ViewCreateProductModel model, int userId)

        {
            var product = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Quantity = model.Quantity,
                BrandName = model.BrandName,
                CreatedAt = DateTime.Now,
                Price = model.Price,  // Thêm giá bán
                UserId = userId,
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
                .Include(x => x.User).Include(x => x.Category)
                .Where(p => p.ProductId == productId)
                .Select(p => new ProductDetailViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    BrandName = p.BrandName,
                    CreatedAt = p.CreatedAt,
                    Price = p.Price,
                    User = p.User,  // Gán đối tượng User đầy đủ
                    CategoryId = p.CategoryId,
                    // Thêm CategoryId ở đây
                    CategoryName = p.Category.CategoryName, // Ánh xạ CategoryName vào ProductDetailViewModel
                    ImageUrls = p.ProductImages.Select(i => i.ImageUrl).ToList()
                })
                .FirstOrDefaultAsync();

            return product;
        }

        //public async Task<ProductDetailViewModel> GetProductWithImagesAsync1(int productId)
        //{
        //    var product = await _unitOfWork.ProductRepository.GetQuery()
        //        .Include(x => x.User).Include(x => x.Category)
        //        .Where(p => p.ProductId == productId)
        //        .Select(p => new ProductDetailViewModel
        //        {1
        //            ProductId = p.ProductId,
        //            ProductName = p.ProductName,
        //            Description = p.Description,
        //            Quantity = p.Quantity,
        //            BrandName = p.BrandName,
        //            CreatedAt = p.CreatedAt,
        //            Price = p.Price,
        //            User = p.User,  // Gán đối tượng User đầy đủ
        //            ImageUrls = p.ProductImages.Select(i => i.ImageUrl).ToList()
        //        })
        //        .FirstOrDefaultAsync();

        //    return product;
        //}


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








        //public async Task<bool> UpdateProductAsync(int productId, ViewCreateProductModel model, List<IFormFile> newImages)
        //{
        //    // Tìm sản phẩm trong cơ sở dữ liệu
        //    var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
        //    if (product == null)
        //    {
        //        return false;
        //    }

        //    // Cập nhật thông tin sản phẩm
        //    product.ProductName = model.ProductName;
        //    product.Description = model.Description;
        //    product.Quantity = model.Quantity;
        //    product.BrandName = model.BrandName;
        //    product.Price = model.Price;
        //    product.CategoryId = model.CategoryId;

        //    _unitOfWork.ProductRepository.Update(product);
        //    await _unitOfWork.SaveChangesAsync();

        //    // Xóa tất cả ảnh cũ của sản phẩm
        //    var oldImages = await _unitOfWork.ProductImageRepository.GetQuery()
        //        .Where(pi => pi.ProductId == productId)
        //        .ToListAsync();
        //    foreach (var image in oldImages)
        //    {
        //        _unitOfWork.ProductImageRepository.Delete(image);

        //        // Xóa file ảnh khỏi thư mục
        //        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));
        //        if (File.Exists(filePath))
        //        {
        //            File.Delete(filePath);
        //        }
        //    }

        //    await _unitOfWork.SaveChangesAsync();

        //    // Lưu các ảnh mới
        //    var newImageUrls = await ValidateAndSaveImagesAsync(productId, newImages);
        //    foreach (var imageUrl in newImageUrls)
        //    {
        //        await SaveProductImageAsync(productId, imageUrl);
        //    }

        //    return true;
        //}




        public async Task<bool> UpdateProductAsync(int productId, ViewCreateProductModel model, List<IFormFile> newImages, List<string> imagesToKeep)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return false;
            }

            // Cập nhật thông tin sản phẩm
            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Quantity = model.Quantity;
            product.BrandName = model.BrandName;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;

            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            // Xóa các ảnh không còn trong danh sách giữ lại
            var oldImages = await _unitOfWork.ProductImageRepository.GetQuery()
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            foreach (var image in oldImages)
            {
                if (!imagesToKeep.Contains(image.ImageUrl))
                {
                    _unitOfWork.ProductImageRepository.Delete(image);

                    // Xóa file ảnh khỏi thư mục
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            // Lưu các ảnh mới
            var newImageUrls = await ValidateAndSaveImagesAsync(productId, newImages);
            foreach (var imageUrl in newImageUrls)
            {
                await SaveProductImageAsync(productId, imageUrl);
            }

            return true;
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
                    Price = (decimal)p.Price,
                    UserId = p.User.UserId,
                    SellerName = p.User.Fullname,
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
                        Price = (decimal)p.Price,
                        UserId = p.User.UserId,
                        SellerName = p.User.Fullname,
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
                    UserId = p.User.UserId,
                    SellerName = p.User.Fullname,
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
                        UserId = p.User.UserId,
                        SellerName = p.User.Fullname,
                        ImageUrls = p.ProductImages.Select(i => i.ImageUrl).Take(1).ToList(),
                        CategoryName = p.Category != null ? p.Category.CategoryName : "No Category"
                    })
                    .ToListAsync();

            return products;
        }

        public async Task<decimal> GetProductPriceById(int productId)
        {
            var products = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            return (decimal)products.Price;
        }
        public async Task UpdateProductQuantityAsync(int productId, int quantity)
        {
            var product = await _unitOfWork.ProductRepository.GetByIdAsync(productId);
            if (product != null)
            {
                product.Quantity -= quantity;

                // Make sure quantity doesn't go below zero
                product.Quantity = Math.Max(0, product.Quantity);

                _unitOfWork.ProductRepository.Update(product);
                await _unitOfWork.SaveChangesAsync();
            }
        }



    }
}
