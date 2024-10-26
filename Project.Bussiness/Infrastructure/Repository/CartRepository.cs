using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure.Repository
{
 
    public class CartRepository : BaseRepository<Cart, EbayClone1Context>
    {
        public CartRepository(EbayClone1Context context) : base(context)
        {
        }

    }


}
