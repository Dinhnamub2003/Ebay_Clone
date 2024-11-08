using System;
using System.Collections.Generic;

namespace Project.Data.Models
{
    public partial class Wallet
    {
        public Wallet()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int WalletId { get; set; }
        public int? UserId { get; set; }
        public decimal? Balance { get; set; }
        public DateTime? LastUpdated { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
