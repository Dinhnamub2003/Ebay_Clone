﻿using System;
using System.Collections.Generic;

namespace Project.Data.Models
{
    public partial class Cart
    {
        public int CartId { get; set; }
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Product? Product { get; set; }
        public virtual User? User { get; set; }
    }
}
