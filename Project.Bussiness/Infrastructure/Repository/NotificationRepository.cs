using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure.Repository
{
   

    public class NotificationRepository : BaseRepository<Notification, EbayClone1Context>
    {
        public NotificationRepository(EbayClone1Context context) : base(context)
        {
        }

    }

}
