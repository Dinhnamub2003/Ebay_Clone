
using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Model.CartModel
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
		public int? AvailableQuantity { get; set; }
		public bool? IsDeleted { get; set; }
		public decimal Price { get; set; }
		public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? Brand { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public virtual Product? Product { get; set; }
        public virtual User? User { get; set; }


    }
}
