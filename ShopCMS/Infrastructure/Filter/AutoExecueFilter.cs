using CoreLib;
using Domain;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using UnitOfWork;

//namespace TfShop.Infrastructure.Filter
//{


//    public class AutoExecueFilter : System.Web.Mvc.ActionFilterAttribute
//    {

//        public override void OnActionExecuted(ActionExecutedContext filterContext)
//        {

//            TfShop.Infrastructure.Cart.order inforder = new TfShop.Infrastructure.Cart.order();
//            UnitOfWorkClass uow = new UnitOfWorkClass();
//            DateTime nowdateTime = DateTime.Now;
//            var ShoppingPayEstelamMinutes = uow.SettingRepository.Get(s => s.ShoppingPayEstelamMinutes, s => s.LanguageId == 1).SingleOrDefault();


//            #region حذف وضعیت استعلامی ها
//            if (uow.OrderRepository.Any(x => x.IsOld == false && x.OrderStates.Last().state == Domain.OrderStatus.تایید_سفارش && x.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value.ToLower() == "true")) && x.OrderStates.Last().LogDate.AddMinutes(ShoppingPayEstelamMinutes) <= DateTime.Now))
//            {
//                var orders = uow.OrderRepository.GetByReturnQueryable(x => x, x => x.IsOld == false, null, "OrderStates,OrderRows,OrderWallets,OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,OrderAttributeSelects.OrderAttribute,OrderDeliveries");

//                orders = orders.Where(x => x.OrderStates.Last().state == Domain.OrderStatus.تایید_سفارش && x.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value == "True")) && x.OrderStates.Last().LogDate.AddMinutes(ShoppingPayEstelamMinutes) <= DateTime.Now);

//                foreach (var item in orders)
//                {
//                    foreach (var item2 in item.OrderDeliveries)
//                    {

//                        uow.OrderStateRepository.Delete(item2.OrderStates.Last());
//                        uow.OrderStateRepository.Insert(new Domain.OrderState { OrderId = item2.OrderId.Value, OrderDeliveryId = item2.Id, Description = "لغو مرسوله به دلیل عدم پرداخت سفارش استعلامی در زمان معین", LogDate = DateTime.Now, state = Domain.OrderStatus.لغو_شده });
//                    }

//                    inforder.CheckProductPriceDefault(item.OrderRows.Select(x => x.ProductId).ToList());
//                    //uow.ProductRepository.CheckProductPriceDefault(item.OrderRows.Select(x => x.ProductId).ToList());

//                    //var ConfirmEstelam = item.OrderAttributeSelects.Where(x => x.OrderAttribute.DataType == 23 && x.Value == "True").SingleOrDefault();
//                    //uow.OrderAttributeOrderRepository.Delete(ConfirmEstelam);

//                }
//                uow.Save();

//            }
//            #endregion

//            #region غیر فعال سازی و لغو سفارشات پرداخت نشده
//            SmsService ss = new SmsService();
//            IdentityMessage iMessage = new IdentityMessage();
//            var expiredOrders = uow.OrderRepository.Get(x => x, x => x.IsOld == false && x.IsExpire == false && (x.ExpireDate != null && x.ExpireDate <= nowdateTime) && !x.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value.ToLower() == "true")) && x.OrderStates.Any(s => s.state == Domain.OrderStatus.تایید_سفارش) && !x.OrderStates.Any(s => s.state == Domain.OrderStatus.تایید_پرداخت), null, "OrderDeliveries,OrderStates,OrderRows,User");

//            string websitename = uow.SettingRepository.Get(x => x.WebSiteName, x => x.LanguageId == 1).SingleOrDefault();
//            foreach (var item in expiredOrders)
//            {
//                item.IsExpire = true;
//                item.New = false;
//                foreach (var delivery in item.OrderDeliveries)
//                {
//                    item.OrderStates.Add(new Domain.OrderState()
//                    {
//                        LogDate = DateTime.Now,
//                        OrderDeliveryId = delivery.Id,
//                        state = Domain.OrderStatus.لغو_شده
//                    });
//                }

//                iMessage.Destination = item.User.PhoneNumber;
//                iMessage.Body = Infrastructure.Sms.Pattern.GetSmsBody(SmsPatternType.تایید_لغو_سفارش, item.UserId, item.CustomerOrderId, "https://www.tfshops.com/Profile/Detail/" + item.CustomerOrderId, "https://www.tfshops.com/Profile/Rate/" + item.CustomerOrderId, null, null, null, OrderStatus.درخواست_لغو.EnumDisplayNameFor(), null, null, null, "https://www.tfshops.com/Profile/Edit");
//                if (!String.IsNullOrEmpty(iMessage.Body))
//                    ss.SendSMSAsync(iMessage, "", null, null, null, null, null, true);


//                uow.OrderRepository.Update(item);

//                //update quantity

//                inforder.CheckQuantity(item);
//                //uow.OrderRepository.CheckQuantity(item);

//                inforder.CheckProductPriceDefault(item.OrderRows.Select(x => x.ProductId).ToList());
//                //uow.ProductRepository.CheckProductPriceDefault(item.OrderRows.Select(x => x.ProductId).ToList());
//            }
//            uow.Save();
//            #endregion
//        }
//    }

//}