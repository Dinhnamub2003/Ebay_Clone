using Microsoft.AspNetCore.Http;
using Project.Data.Models;
using Project.Model.UserModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Model.ProductModel
{
    public class ViewCreateProductModel
    {
        [Required]
        public string ProductName { get; set; } = null!;

        [Required(ErrorMessage = "Description must not empty.")]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        public string? BrandName { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Please enter a price.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; } // Giá bán

        // Lưu danh sách ảnh upload
        public List<IFormFile>? Images { get; set; }

     
    }

}
