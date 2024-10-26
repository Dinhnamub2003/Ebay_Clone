using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure.Repository
{
   



    public class UserRepository : BaseRepository<User, EbayClone1Context>
    {
        public UserRepository(EbayClone1Context context) : base(context)
        {
        }

    }

}
