using Project.Data.Models;
using System;
using System.Collections.Generic;

namespace Project.Model.ProductModel
{
    public class ProductDetailViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string? BrandName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal? Price { get; set; }  // Thêm trường giá
        public string? PostedBy { get; set; }  // Thêm trường người đăng
        public List<string> ImageUrls { get; set; } = new List<string>();

        // Thêm thuộc tính User vào ProductDetailViewModel
        public int? CategoryId { get; set; } // Thêm thuộc tính CategoryId
        public string? CategoryName { get; set; } // Thêm thuộc tính CategoryName để lưu tên danh mục

        public User? User { get; set; }
    }
}
