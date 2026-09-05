using DataLayer;
using Domain;
using System;

namespace Repository.Service
{
    /// <summary>
    /// برای ایجاد  یک ایدی منحصر به فرد برای فرستادن به سمت بانک
    /// </summary>
    public class CreateOrderKeyService : GenericRepository<CreateOrderKey>
    {
        public CreateOrderKeyService(TfShopDbContext context) : base(context)
        {
        }

        public long GetOrderId(Guid? id = null)
        {
            var model = new CreateOrderKey();
            if (id.HasValue)
                model.OrderId = id.Value;
            Insert(model);
            context.SaveChanges();
            return model.Id;
        }
        public long updateGetOrderId(long id, Guid orderid)
        {
            var model = context.CreateOrderKeies.Find(id);
            model.OrderId = orderid;
            Update(model);
            context.SaveChanges();
            return model.Id;
        }
    }
}
