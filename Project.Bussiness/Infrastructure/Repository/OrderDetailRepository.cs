using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure.Repository
{
   
    public class OrderDetailRepository : BaseRepository<OrderDetail, EbayClone1Context>
    {
        public OrderDetailRepository(EbayClone1Context context) : base(context)
        {
        }

    }

}
