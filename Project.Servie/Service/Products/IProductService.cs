﻿using Microsoft.AspNetCore.Http;
using Project.Model.ProductModel;
using System.Threading.Tasks;

namespace Project.Servie.Service.Products
{
    public interface IProductService
    {
        Task<int> AddProductAsync(ViewCreateProductModel model);
        Task SaveProductImageAsync(int productId, string imageUrl);
        Task<ProductDetailViewModel> GetProductWithImagesAsync(int productId);
        Task<List<string>> ValidateAndSaveImagesAsync(int productId, List<IFormFile> images);
    }
}
