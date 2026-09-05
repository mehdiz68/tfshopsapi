using DataLayer;
using Domain;

namespace Repository.Service
{
    public class BankAccountOnlineInfoService : GenericRepository<BankAccountOnlineInfo>
    {
        public BankAccountOnlineInfoService(TfShopDbContext context) : base(context)
        {
        }
    }
}
