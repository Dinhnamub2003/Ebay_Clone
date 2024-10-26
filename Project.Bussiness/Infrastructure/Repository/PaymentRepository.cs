using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure.Repository
{
   


    public class PaymentRepository : BaseRepository<Payment, EbayClone1Context>
    {
        public PaymentRepository(EbayClone1Context context) : base(context)
        {
        }

    }
}
