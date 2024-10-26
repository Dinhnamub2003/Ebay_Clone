using System;
using System.Collections.Generic;

namespace Project.Data.Models
{
    public partial class ProductImage
    {
        public int ImageId { get; set; }
        public int? ProductId { get; set; }
        public string ImageUrl { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Product? Product { get; set; }
    }
}
