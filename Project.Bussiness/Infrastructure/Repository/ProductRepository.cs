using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure.Repository
{
  



    public class ProductRepository : BaseRepository<Product, EbayClone1Context>
    {
        public ProductRepository(EbayClone1Context context) : base(context)
        {
        }

    }


}
