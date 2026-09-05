using DataLayer;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class ProductLetmeKnowService : GenericRepository<ProductLetmeknow>
    {
        private readonly ProductService _productService;
        public ProductLetmeKnowService(TfShopDbContext context, ProductService productService) : base(context)
        {
            this._productService = productService;
        }

        /// <summary>
        /// نمایش لیست به من اطلاع بده ی کاربر
        /// </summary>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>
        public IQueryable<ProductLetmeknow> GetList(string userid)
        {
            return GetQueryList().AsNoTracking().Include("Product").Include("ProductPrice").Include("ProductPrice.ProductAttributeSelectModel").Include("ProductPrice.ProductAttributeSelectSize").Include("ProductPrice.ProductAttributeSelectColor").Include("Product.ProductImages.Image").Include("ProductPrice.ProductImages.Image").Include("ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Where(x => x.UserId == userid && (x.Notofied == false || x.NotofiedEmail == false || x.NotofiedSms == false || x.NotofiedAmazing == false || x.NotofiedEmailAmazing == false || x.NotofiedSmsAmazing == false)).OrderByDescending(x=>x.InsertDate);

        }

        /// <summary>
        /// متد بررسی موجود بودن محصول در لیست به من اطلاع بده
        /// </summary>
        /// <param name="productid">کد محصول</param>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>

        public int? checkUserProductletmeKnow(int ProductPriceId, string userid)
        {
            return Get(x => x.Id, x => x.ProductPriceId == ProductPriceId && x.UserId == userid, null, "", 0, 0, true).FirstOrDefault();

        }
        /// <summary>
        /// حذف به من اطلاع بده ی کاربر
        /// </summary>
        /// <param name="id">ردیف به من اطلاع بده</param>
        /// <returns></returns>
        public async Task removeLetmeKnow(int ProductPriceId)
        {
            Delete(dbSet.Where(x => x.Id == ProductPriceId).FirstOrDefault());
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// افزودن محصوله به لیست به من اطلاع بده ی کاربر
        /// </summary>
        /// <param name="productId">کد محصول</param>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>
        public async Task addLetmeKnow(int productId, int productPriceId, string userid, bool amazingoffer, bool available, short notificationType)
        {
            var plmn = Get(x => x, x => x.ProductPriceId == productPriceId && x.UserId == userid).FirstOrDefault();
            if (plmn == null)
            {
                Insert(new ProductLetmeknow()
                {
                    InsertDate = DateTime.Now,
                    ProductId = productId,
                    ProductPriceId = productPriceId,
                    UserId = userid,
                    AmazingOffer = amazingoffer,
                    Available = available,
                    NotificationType = notificationType
                });
            }
            else
            {
                plmn.InsertDate = DateTime.Now;
                plmn.AmazingOffer = amazingoffer;
                plmn.Available = available;
                plmn.NotificationType = notificationType;
                Update(plmn);
            }



            await context.SaveChangesAsync();
        }

        public async Task<bool> CheckLetmeKnowsOfProduct(int ProductPriceId)
        {
            return await context.ProductLetmeknows.AnyAsync(x => x.ProductPriceId == ProductPriceId);
        }
        public async Task<List<Domain.ProductLetmeknow>> GetLetmeKnowsOfProduct(int ProductPriceId)
        {
            return await context.ProductLetmeknows.Where(x => x.ProductPriceId == ProductPriceId).ToListAsync();
        }
        public async Task removeNotifications(int productId)
        {

        }
    }
}
