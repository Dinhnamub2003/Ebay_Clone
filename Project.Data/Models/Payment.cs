using System;
using System.Collections.Generic;

namespace Project.Data.Models
{
    public partial class Payment
    {
        public int PaymentId { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentStatus { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Order? Order { get; set; }
        public virtual User? User { get; set; }
    }
}
