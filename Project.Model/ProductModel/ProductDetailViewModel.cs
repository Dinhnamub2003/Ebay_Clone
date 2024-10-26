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
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
