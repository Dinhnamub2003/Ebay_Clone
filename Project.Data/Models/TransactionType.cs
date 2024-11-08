using System;
using System.Collections.Generic;

namespace Project.Data.Models
{
    public partial class TransactionType
    {
        public TransactionType()
        {
            Transactions = new HashSet<Transaction>();
        }

        public int TransactionTypeId { get; set; }
        public string TypeName { get; set; } = null!;

        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
