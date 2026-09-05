using DataLayer;
using Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class UserCodeGiftService : GenericRepository<UserCodeGift>
    {
        public UserCodeGiftService(TfShopDbContext context) : base(context)
        {

        }
       
    }
}
