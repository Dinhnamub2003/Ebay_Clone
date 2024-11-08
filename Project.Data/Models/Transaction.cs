using System;
using System.Collections.Generic;

namespace Project.Data.Models
{
    public partial class Transaction
    {
        public int TransactionsId { get; set; }
        public int WalletId { get; set; }
        public int TransactionTypeId { get; set; }
        public string? Bank { get; set; }
        public string? ImgBank { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string Status { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual TransactionType TransactionType { get; set; } = null!;
        public virtual Wallet Wallet { get; set; } = null!;
    }
}
