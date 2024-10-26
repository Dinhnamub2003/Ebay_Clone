using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure.Repository
{
   


    public class ProductImageRepository : BaseRepository<Product, EbayClone1Context>
    {
        public ProductImageRepository(EbayClone1Context context) : base(context)
        {
        }

    }


}
