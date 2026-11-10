using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using UnitOfWork;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TfShop.Controllers
{
    public class ZarehbinController : ApiController
    {
        UnitOfWorkClass uow = null;
        public ZarehbinController()
        {
            uow = new UnitOfWorkClass();
        }
        // فیدِ ذره‌بین (از Tfshop/ShopCMS منتقل شد). کالایی که همه‌ی گروه‌هایش مخفی‌اند نمی‌رود - همان قاعده‌ی ترب و جستجویِ سایت
        [HttpGet]

        public async Task<JsonResult<Domain.ViewModels.ZarehbinItem>> products(int? product_id, int? page)
        {
            try
            {

                if (page.HasValue)
                {
                    int pg = page.Value;
                    int skip = (pg - 1) * 100;
                    return Json(uow.ProductRepository.ZarebinListPagging(x => x.IsActive && x.ProductPrices.Any(p => p.IsDefault) && x.state == 4 && !(x.ProductCategories.Any(c => c.hidden) && !x.ProductCategories.Any(c => !c.hidden)), x => x.OrderByDescending(o => o.Id), skip, 100));
                }
                else if (product_id.HasValue)
                {
                    int id = 0;
                    if (product_id.HasValue)
                        id = product_id.Value;
                    return Json(uow.ProductRepository.ZarebinList(x => x.Id == id && x.IsActive && x.ProductPrices.Any(p => p.IsDefault) && x.state == 4 && !(x.ProductCategories.Any(c => c.hidden) && !x.ProductCategories.Any(c => !c.hidden)), x => x.OrderByDescending(o => o.Id)));

                }
                else
                {
                    int pg = 1;
                    int skip = (pg - 1) * 100;
                    return Json(uow.ProductRepository.ZarebinListPagging(x => x.IsActive && x.ProductPrices.Any(p => p.IsDefault) && x.state == 4 && !(x.ProductCategories.Any(c => c.hidden) && !x.ProductCategories.Any(c => !c.hidden)), x => x.OrderByDescending(o => o.Id), skip, 100));

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }

}
