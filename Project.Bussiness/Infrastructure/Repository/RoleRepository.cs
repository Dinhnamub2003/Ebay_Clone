﻿using Project.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Bussiness.Infrastructure.Repository
{
  



    public class RoleRepository : BaseRepository<Role, EbayClone1Context>
    {
        public RoleRepository(EbayClone1Context context) : base(context)
        {
        }

    }
}
