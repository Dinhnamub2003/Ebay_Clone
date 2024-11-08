using System;
using System.Collections.Generic;

namespace Project.Data.Models
{
    public partial class User
    {
        public User()
        {
            Carts = new HashSet<Cart>();
            Notifications = new HashSet<Notification>();
            Orders = new HashSet<Order>();
            Payments = new HashSet<Payment>();
            Products = new HashSet<Product>();
            Reviews = new HashSet<Review>();
        }

        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public string? Code { get; set; }
        public string? Avatar { get; set; }
        public bool? IsVerification { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Role? Role { get; set; }
        public virtual Wallet? Wallet { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
