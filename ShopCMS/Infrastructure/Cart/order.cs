using Domain;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UnitOfWork;

namespace TfShop.Infrastructure.Cart
{
    public class order
    {
        UnitOfWorkClass uow = null;
        public bool UpdateQuantity(Order order)
        {
            using (uow = new UnitOfWorkClass())
            {
                try
                {
                    List<int> ids = new List<int>();
                    //update quantity
                    foreach (var item in order.OrderRows.Where(x => x.Decrease == false))
                    {
                        var prp = uow.ProductPriceRepository.GetByID(item.ProductPriceId);
                        prp.Quantity -= item.Quantity;
                        //prp.MaxBasketCount -= item.Quantity;
                        //ناموجود شود
                        if (prp.Quantity < 1)
                        {
                            prp.Quantity = 0;
                            prp.ProductStateId = 4;
                            ////اگر تنوع پیش فرض می باشد، محصول ناموجود شود
                            //if (prp.IsDefault)
                            //{
                            //    var pr = context.Products.Find(item.ProductId);
                            //    pr.ProductStateId = 5;
                            //}
                        }
                        //if (prp.MaxBasketCount < 1)
                        //    prp.MaxBasketCount = 0;
                        uow.ProductPriceRepository.Update(prp);
                        uow.Save();

                        var proffer = uow.ProductOfferRepository.Get(a => a, a => a.ProductPriceId == item.ProductPriceId && a.Offer.IsActive && a.Offer.state == true && ((a.Offer.ExpireDate != null && a.Offer.ExpireDate >= DateTime.Now) || a.Offer.ExpireDate == null) && ((a.Offer.StartDate != null && a.Offer.StartDate <= DateTime.Now) || a.Offer.StartDate == null)).FirstOrDefault();
                        if (proffer != null)
                        {
                            proffer.Quantity -= item.Quantity;
                            //proffer.MaxBasketCount -= item.Quantity;
                            if (proffer.Quantity < 1)
                                proffer.Quantity = 0;

                            //if (proffer.MaxBasketCount < 1)
                            //    proffer.MaxBasketCount = 0;

                            uow.ProductOfferRepository.Update(proffer);
                            uow.Save();
                        }
                        ids.Add(item.Id);

                    }
                    var orderRows = uow.OrderRowRepository.Get(x => x, x => ids.Contains(x.Id));
                    orderRows.ForEach(x => x.Decrease = true);
                    foreach (var item in orderRows)
                        uow.OrderRowRepository.Update(item);
                    uow.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    TfShop.Infrastructure.EventLog.Logger.Add(5, "order", "UpdateQuantity", false, 500, ex.Message + ex.InnerException != null ? ex.InnerException.Message : "", DateTime.Now, uow.UserRepository.GetQueryList().FirstOrDefault().Id);

                    return false;
                }
            }
        }
        public bool CheckQuantity(Order order)
        {
            using (uow = new UnitOfWorkClass())
            {
                try
                {
                    List<int> ids = new List<int>();
                    //update quantity
                    foreach (var item in order.OrderRows.Where(x => x.Increase == false))
                    {
                        var prp = uow.ProductPriceRepository.GetByID(item.ProductPriceId);
                        prp.Quantity += item.Quantity;
                        //موچود شود
                        if (prp.Quantity > 0 && prp.ProductStateId > 2)
                        {
                            prp.ProductStateId = 1;
                            ////اگر تنوع پیش فرض می باشد، محصول موجود شود
                            //if (prp.IsDefault)
                            //{
                            //    var pr = context.Products.Find(item.ProductId);
                            //    pr.ProductStateId = 1;
                            //    context.SaveChanges();
                            //}
                        }
                        uow.ProductPriceRepository.Update(prp);
                        uow.Save();

                        var proffer = uow.ProductOfferRepository.Get(a => a, a => a.ProductPriceId == item.ProductPriceId && a.Offer.IsActive && a.Offer.state == true && ((a.Offer.ExpireDate != null && a.Offer.ExpireDate >= DateTime.Now) || a.Offer.ExpireDate == null) && ((a.Offer.StartDate != null && a.Offer.StartDate <= DateTime.Now) || a.Offer.StartDate == null)).FirstOrDefault();
                        if (proffer != null)
                        {
                            proffer.Quantity += item.Quantity;
                            uow.ProductOfferRepository.Update(proffer);
                            uow.Save();
                        }

                    }
                    var orderRows = uow.OrderRowRepository.Get(x => x, x => ids.Contains(x.Id));
                    orderRows.ForEach(x => x.Increase = true);
                    foreach (var item in orderRows)
                        uow.OrderRowRepository.Update(item);
                    uow.Save();
                    return true;
                }
                catch (Exception ex)
                {
                    TfShop.Infrastructure.EventLog.Logger.Add(5, "order", "CheckQuantity", false, 500, ex.Message + ex.InnerException != null ? ex.InnerException.Message : "", DateTime.Now, uow.UserRepository.GetQueryList().FirstOrDefault().Id);
                    return false;
                }
            }
            
        }

        public bool CheckProductPriceDefault(List<int> ids)
        {
            using (uow = new UnitOfWorkClass())
            {
                try
                {
                    var ProductPriceOrdersetting = uow.SettingRepository.Get(x => x, x => x.LanguageId == 1).Select(x => new { x.ProductPriceOrderFirst, x.ProductPriceOrderSecond, x.ProductPriceOrderThird, x.ProductPriceOrderfourth }).SingleOrDefault();

                    foreach (var item in uow.ProductRepository.Get(x => x, x => ids.Contains(x.Id), null, "ProductPrices.Product,ProductPrices.ProductAttributeSelectModel,ProductPrices.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ProductPrices.ProductAttributeSelectColor"))
                    {
                        //if (!item.ProductPrices.Any(x => x.IsDefault) || item.ProductPrices.Any(x => x.IsDefault && x.ProductStateId == 5))
                        {
                            #region setDefault
                            foreach (var item2 in item.ProductPrices)
                            {
                                item2.IsDefault = false;
                                uow.ProductPriceRepository.Update(item2);
                            }
                            uow.Save();

                            var prps = item.ProductPrices.AsQueryable().OrderByDescending(x => x.Id);
                            switch (ProductPriceOrdersetting.ProductPriceOrderFirst)
                            {
                                case ProductPriceOrder.قیمت:
                                    prps = prps.OrderBy(x => x.Price);
                                    break;
                                case ProductPriceOrder.وضعیت_موجودی:
                                    prps = prps.OrderBy(x => x.ProductStateId);
                                    break;
                                case ProductPriceOrder.موجودی:
                                    prps = prps.OrderByDescending(x => x.Quantity);
                                    break;
                                case ProductPriceOrder.بازه_زمانی_ارسال:
                                    prps = prps.OrderBy(x => x.DeliveryTimeout);
                                    break;
                                default:
                                    break;
                            }
                            switch (ProductPriceOrdersetting.ProductPriceOrderSecond)
                            {
                                case ProductPriceOrder.قیمت:
                                    prps = prps.ThenBy(x => x.Price);
                                    break;
                                case ProductPriceOrder.وضعیت_موجودی:
                                    prps = prps.ThenBy(x => x.ProductStateId);
                                    break;
                                case ProductPriceOrder.موجودی:
                                    prps = prps.ThenByDescending(x => x.Quantity);
                                    break;
                                case ProductPriceOrder.بازه_زمانی_ارسال:
                                    prps = prps.ThenBy(x => x.DeliveryTimeout);
                                    break;
                                default:
                                    break;
                            }
                            switch (ProductPriceOrdersetting.ProductPriceOrderThird)
                            {
                                case ProductPriceOrder.قیمت:
                                    prps = prps.ThenBy(x => x.Price);
                                    break;
                                case ProductPriceOrder.وضعیت_موجودی:
                                    prps = prps.ThenBy(x => x.ProductStateId);
                                    break;
                                case ProductPriceOrder.موجودی:
                                    prps = prps.ThenByDescending(x => x.Quantity);
                                    break;
                                case ProductPriceOrder.بازه_زمانی_ارسال:
                                    prps = prps.ThenBy(x => x.DeliveryTimeout);
                                    break;
                                default:
                                    break;
                            }
                            switch (ProductPriceOrdersetting.ProductPriceOrderfourth)
                            {
                                case ProductPriceOrder.قیمت:
                                    prps = prps.ThenBy(x => x.Price);
                                    break;
                                case ProductPriceOrder.وضعیت_موجودی:
                                    prps = prps.ThenBy(x => x.ProductStateId);
                                    break;
                                case ProductPriceOrder.موجودی:
                                    prps = prps.ThenByDescending(x => x.Quantity);
                                    break;
                                case ProductPriceOrder.بازه_زمانی_ارسال:
                                    prps = prps.ThenBy(x => x.DeliveryTimeout);
                                    break;
                                default:
                                    break;
                            }
                            var defalutProductPrice = prps.First();
                            defalutProductPrice.IsDefault = true;
                            uow.ProductPriceRepository.Update(defalutProductPrice);
                            uow.Save();

                            string title = defalutProductPrice.Product.Name;
                            string pageaddress = defalutProductPrice.Product.Name;
                            if (defalutProductPrice.ProductAttributeSelectModelId.HasValue)
                            {
                                title += " مدل " + defalutProductPrice.ProductAttributeSelectModel.Value;
                                pageaddress += " " + defalutProductPrice.ProductAttributeSelectModel.Value;
                            }
                            if (defalutProductPrice.ProductAttributeSelectSizeId.HasValue)
                                title += " سایز " + defalutProductPrice.ProductAttributeSelectSize.Value + defalutProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit;
                            if (defalutProductPrice.ProductAttributeSelectColorId.HasValue)
                                title += " " + uow.ProductAttributeItemColorRepository.GetByID(int.Parse(uow.ProductAttributeSelectRepository.GetByID(defalutProductPrice.ProductAttributeSelectColorId).Value)).Color;
                            var product = defalutProductPrice.Product;
                            product.PageAddress = CoreLib.Infrastructure.CommonFunctions.NormalizeAddressWithSpace(pageaddress);
                            product.Title = title;

                            uow.ProductRepository.Update(product);
                            uow.Save();


                            #endregion
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    TfShop.Infrastructure.EventLog.Logger.Add(5, "order", "CheckProductPriceDefault", false, 500, ex.Message + ex.InnerException != null ? ex.InnerException.Message : "", DateTime.Now, uow.UserRepository.GetQueryList().FirstOrDefault().Id);

                    return false;
                }
            }
        }
    }
}