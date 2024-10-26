using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure.Repository
{
 
    public class OrderRepository : BaseRepository<Order, EbayClone1Context>
    {
        public OrderRepository(EbayClone1Context context) : base(context)
        {
        }

    }
}
