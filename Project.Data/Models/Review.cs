using System;
using System.Collections.Generic;

namespace Project.Data.Models
{
    public partial class Review
    {
        public int ReviewId { get; set; }
        public int? ProductId { get; set; }
        public int? ReviewerId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? ReviewDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Product? Product { get; set; }
        public virtual User? Reviewer { get; set; }
    }
}
