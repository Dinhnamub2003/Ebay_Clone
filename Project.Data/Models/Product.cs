using System;
using System.Collections.Generic;

namespace Project.Data.Models
{
    public partial class Product
    {
        public Product()
        {
            Carts = new HashSet<Cart>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductImages = new HashSet<ProductImage>();
            Reviews = new HashSet<Review>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public string? BrandName { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
        public decimal? Price { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Category? Category { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
