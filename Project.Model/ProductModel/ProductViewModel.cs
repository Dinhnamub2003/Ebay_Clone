using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.ProductModel
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string? BrandName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? FirstImageUrl { get; set; }
        public string? CategoryName { get; set; }
		public decimal Price { get; set; }
        public int UserId { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
