using DataLayer;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Repository.Service
{
    public class OrderService : GenericRepository<Order>
    {
        private readonly OrderStateService _orderStateService;
        private readonly ProductPriceService _productPriceService;
        private readonly OrderAttributeOrderService _orderAttributeOrderService;
        private readonly ProductService _productService;
        private readonly CreateOrderKeyService _createOrderKeyService;
        public OrderService(TfShopDbContext context, OrderStateService orderStateService, ProductPriceService productPriceService, OrderAttributeOrderService orderAttributeOrderService, ProductService productService, CreateOrderKeyService createOrderKeyService) : base(context)
        {

            this._orderStateService = orderStateService;
            this._productPriceService = productPriceService;
            this._productService = productService;
            this._orderAttributeOrderService = orderAttributeOrderService;
            this._createOrderKeyService = createOrderKeyService;
        }
        public IEnumerable<Order> GetExpiredOrders()
        {
            DateTime nowdateTime = DateTime.Now;
            return Get(x => x, x => x.IsOld == false && x.IsExpire == false && (x.ExpireDate != null && x.ExpireDate <= nowdateTime) && !x.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(r => r.WalletAttribute.DataType == 23 && r.Value.ToLower() == "true")) && x.OrderStates.Any(s => s.state == Domain.OrderStatus.تایید_سفارش) && !x.OrderStates.Any(s => s.state == Domain.OrderStatus.تایید_پرداخت) && !x.OrderStates.Any(s => s.state == Domain.OrderStatus.لغو_شده), x => x.OrderBy(s => s.BankOrderId), "OrderDeliveries,OrderStates,OrderRows,User", 0, 5);
        }
        public IQueryable<Order> GetAllOrder(string userid, string date, string keyword)
        {
            var orders = GetQueryList().AsNoTracking().Include("OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image").Include("OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel").Include("OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize").Include("OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor").Include("OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").Include("OrderStates").Include("OrderDeliveries").Include("OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image").Include("OrderWallets.Wallet.BankAccount").Include("OrderRates").Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image").Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel").Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize").Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor").Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("ChildOrders.OrderDeliveries").Include("ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image").Include("ChildOrders.OrderRows.ProductPrice.ProductImages.Image").Include("ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectModel").Include("ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize").Include("ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectColor").Include("ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("ChildOrders.OrderDeliveries").Include("ChildOrders.OrderRows.Product.ProductPrices.ProductImages.Image").Where(x => x.UserId == userid);
            if (!String.IsNullOrEmpty(date))
            {
                DateTime dt = DateTime.Now.AddDays(-31);
                if (date == "2")
                    dt = DateTime.Now.AddDays(-90);
                else if (date == "3")
                    dt = DateTime.Now.AddDays(-365);
                orders = orders.Where(x => x.InsertDate >= dt);
            }
            if (!String.IsNullOrEmpty(keyword))
            {
                var color = context.ProductAttributeItemColors.Where(x => x.Color.Contains(keyword)).FirstOrDefault();
                string colorId = "0";
                if (color != null)
                    colorId = color.Id.ToString();
                var Garanty = context.ProductAttributeItems.Where(x => x.Value.Contains(keyword)).FirstOrDefault();
                string GarantyId = "0";
                if (Garanty != null)
                    GarantyId = Garanty.Id.ToString();
                orders = from s in orders
                         where
                         (s.CustomerOrderId == keyword) ||
                                (s.OrderRows.Any(x => x.Product.LatinName != null) && s.OrderRows.Any(x => x.Product.LatinName.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.Product.Name != null) && s.OrderRows.Any(x => x.Product.Name.ToLower().Contains(keyword))) ||
                                 (s.OrderRows.Any(x => x.ProductPrice.code != null) && s.OrderRows.Any(x => x.ProductPrice.code.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModelId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModel.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSizeId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSize.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectWeightId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectweight.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColorId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColor.Value.ToLower() == colorId)) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGarantyId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId))
                         select s;
            }
            return orders.OrderByDescending(s => s.BankOrderId);

        }
        public IQueryable<Order> GetCurrentOrder(string userid, string date, string keyword)
        {
            var orderStates = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,ChildOrders.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderRows.Product.ProductPrices.ProductImages.Image").AsQueryable()
                                  //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                              where x.Order.UserId == userid
                              group x by x.OrderId into g
                              select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orderStates.Where(x => x.state > OrderStatus.تایید_پرداخت && x.state < OrderStatus.تحویل_داده_شده).Select(x => x.Key);

            var orders = GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id) && x.OrderId == null, x => x.OrderByDescending(s => s.BankOrderId), "OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount,OrderRates,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image");
            if (!String.IsNullOrEmpty(date))
            {
                DateTime dt = DateTime.Now.AddDays(-31);
                if (date == "2")
                    dt = DateTime.Now.AddDays(-90);
                else if (date == "3")
                    dt = DateTime.Now.AddDays(-365);
                orders = orders.Where(x => x.InsertDate >= dt);
            }
            if (!String.IsNullOrEmpty(keyword))
            {
                var color = context.ProductAttributeItemColors.Where(x => x.Color.Contains(keyword)).FirstOrDefault();
                string colorId = "0";
                if (color != null)
                    colorId = color.Id.ToString();
                var Garanty = context.ProductAttributeItems.Where(x => x.Value.Contains(keyword)).FirstOrDefault();
                string GarantyId = "0";
                if (Garanty != null)
                    GarantyId = Garanty.Id.ToString();
                orders = from s in orders
                         where
                         (s.CustomerOrderId == keyword) ||
                                (s.OrderRows.Any(x => x.Product.LatinName != null) && s.OrderRows.Any(x => x.Product.LatinName.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.Product.Name != null) && s.OrderRows.Any(x => x.Product.Name.ToLower().Contains(keyword))) ||
                                 (s.OrderRows.Any(x => x.ProductPrice.code != null) && s.OrderRows.Any(x => x.ProductPrice.code.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModelId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModel.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSizeId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSize.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectWeightId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectweight.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColorId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColor.Value.ToLower() == colorId)) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGarantyId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId))
                         select s;
            }
            return orders.OrderByDescending(s => s.BankOrderId);
        }
        public IQueryable<Order> GetProccessOrder(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                         where x.Order.UserId == userid
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.پردازش_انبار).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id) && x.OrderId == null, x => x.OrderByDescending(s => s.BankOrderId), "OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount,OrderRates,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image");
        }
        public IQueryable<Order> GetSentOrder(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                         where x.Order.UserId == userid
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.ارسال_شده).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id) && x.OrderId == null, x => x.OrderByDescending(s => s.BankOrderId), "OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount,OrderRates,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image");
        }
        public IQueryable<Order> GetCancelOrders(string userid, string date, string keyword)
        {
            var orderStates = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,ChildOrders.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderRows.Product.ProductPrices.ProductImages.Image").AsQueryable()
                                  //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                              where x.Order.UserId == userid
                              group x by x.OrderId into g
                              select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orderStates.Where(x => x.state == OrderStatus.لغو_شده || x.state == OrderStatus.درخواست_لغو || x.state == OrderStatus.عدم_تایید_درخواست_لغو).Select(x => x.Key);

            var orders = GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id) && x.OrderId == null, x => x.OrderByDescending(s => s.BankOrderId), "OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount,OrderRates,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image");
            if (!String.IsNullOrEmpty(date))
            {
                DateTime dt = DateTime.Now.AddDays(-31);
                if (date == "2")
                    dt = DateTime.Now.AddDays(-90);
                else if (date == "3")
                    dt = DateTime.Now.AddDays(-365);
                orders = orders.Where(x => x.InsertDate >= dt);
            }
            if (!String.IsNullOrEmpty(keyword))
            {
                var color = context.ProductAttributeItemColors.Where(x => x.Color.Contains(keyword)).FirstOrDefault();
                string colorId = "0";
                if (color != null)
                    colorId = color.Id.ToString();
                var Garanty = context.ProductAttributeItems.Where(x => x.Value.Contains(keyword)).FirstOrDefault();
                string GarantyId = "0";
                if (Garanty != null)
                    GarantyId = Garanty.Id.ToString();
                orders = from s in orders
                         where
                         (s.CustomerOrderId == keyword) ||
                                (s.OrderRows.Any(x => x.Product.LatinName != null) && s.OrderRows.Any(x => x.Product.LatinName.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.Product.Name != null) && s.OrderRows.Any(x => x.Product.Name.ToLower().Contains(keyword))) ||
                                 (s.OrderRows.Any(x => x.ProductPrice.code != null) && s.OrderRows.Any(x => x.ProductPrice.code.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModelId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModel.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSizeId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSize.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectWeightId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectweight.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColorId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColor.Value.ToLower() == colorId)) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGarantyId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId))
                         select s;
            }
            return orders.OrderByDescending(s => s.BankOrderId);
        }

        public IQueryable<Order> GetCancelWaitOrders(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true)
                         where x.Order.UserId == userid && !x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.درخواست_لغو).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id) && x.OrderId == null, x => x.OrderByDescending(s => s.BankOrderId), "OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount,OrderRates,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image");
        }

        public IQueryable<Order> GetDeliveredOrders(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                             //where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.Order.UserId == userid && !x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                         where x.Order.UserId == userid && !x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.تحویل_داده_شده).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id) && x.OrderId == null, x => x.OrderByDescending(s => s.BankOrderId), "OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount,OrderRates,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image");
        }

        public IQueryable<Order> GetReturnedOrders(string userid, string date, string keyword)
        {
            var orderStates = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute,ChildOrders.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderRows.Product.ProductPrices.ProductImages.Image").AsQueryable()
                              where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.Order.UserId == userid && !x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                              group x by x.OrderId into g
                              select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orderStates.Where(x => x.state == OrderStatus.مرجوعی || x.state == OrderStatus.عدم_تایید_درخواست_مرجوعی || x.state == OrderStatus.درخواست_مرجوعی || x.state == OrderStatus.جبران_مرجوعی).Select(x => x.Key);

            var orders = GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id), x => x.OrderByDescending(s => s.BankOrderId), "OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount,OrderRates,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image");
            if (!String.IsNullOrEmpty(date))
            {
                DateTime dt = DateTime.Now.AddDays(-31);
                if (date == "2")
                    dt = DateTime.Now.AddDays(-90);
                else if (date == "3")
                    dt = DateTime.Now.AddDays(-365);
                orders = orders.Where(x => x.InsertDate >= dt);
            }
            if (!String.IsNullOrEmpty(keyword))
            {
                var color = context.ProductAttributeItemColors.Where(x => x.Color.Contains(keyword)).FirstOrDefault();
                string colorId = "0";
                if (color != null)
                    colorId = color.Id.ToString();
                var Garanty = context.ProductAttributeItems.Where(x => x.Value.Contains(keyword)).FirstOrDefault();
                string GarantyId = "0";
                if (Garanty != null)
                    GarantyId = Garanty.Id.ToString();
                orders = from s in orders
                         where
                         (s.CustomerOrderId == keyword) ||
                                (s.OrderRows.Any(x => x.Product.LatinName != null) && s.OrderRows.Any(x => x.Product.LatinName.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.Product.Name != null) && s.OrderRows.Any(x => x.Product.Name.ToLower().Contains(keyword))) ||
                                 (s.OrderRows.Any(x => x.ProductPrice.code != null) && s.OrderRows.Any(x => x.ProductPrice.code.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModelId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModel.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSizeId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSize.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectWeightId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectweight.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColorId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColor.Value.ToLower() == colorId)) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGarantyId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId))
                         select s;
            }
            return orders.OrderByDescending(s => s.BankOrderId);
        }

        public IQueryable<Order> GetEstelamOrders(string userid)
        {
            var orders = from x in _orderStateService.GetQueryList().AsNoTracking().Include("Order").Include("Order.OrderWallets.Wallet,Order.OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute").AsQueryable()
                         where x.Order.UserId == userid && x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                         group x by x.OrderId into g
                         select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orders.Where(x => x.state == OrderStatus.در_انتظار_تایید || x.state == OrderStatus.عدم_تایید_سفارش).Select(x => x.Key);

            return GetByReturnQueryable(x => x, x => currentorder.Contains(x.Id) && x.OrderId == null, x => x.OrderByDescending(s => s.CustomerOrderId), "OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,OrderWallets.Wallet,OrderStates,OrderDeliveries,OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image,OrderWallets.Wallet.BankAccount,OrderRates,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor,ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute,ChildOrders.OrderDeliveries,ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image");
        }

        public long AddOrder(string userid, string adminuserid, List<Basket> BasketItems, List<ProductBox> productBoxes, string userDescription, bool sendFactor, int BankAccountId, int PaymentType, string codeGift, int bon, short languageid, string UserPayAttach, out bool IsEstelam, out bool validFreeSend, out string customid)
        {
            customid = "";
            try
            {

                var cuser = context.Users.Include("UserAddresses.CityEntity.Province").Include("CityEntity.Province").Where(x => x.Id == userid).FirstOrDefault();
                bool UserIsHaghighi = cuser.Haghighi;
                validFreeSend = true;
                IsEstelam = false;
                DateTime dateTime = DateTime.Now;
                var setting = context.Settings.Where(x => x.LanguageId == languageid).FirstOrDefault();
                var ProductBasketItems = _productPriceService.ProductBasketItem(BasketItems);
                bool FreeSendPrice = false;
                bool? code_bon = true;
                long OffPrice = -1;
                int? UsergiftCodeId = null;
                int? giftCodeId = null;
                int? giftListCodeId = null;
                var bankaccount = context.BankAccounts.Find(BankAccountId);
                double hours = 0;
                switch (PaymentType)
                {
                    case 1: case 2: hours = bankaccount.OnliePaymentHours; break;
                    case 3: hours = bankaccount.CardNumberHours; break;
                    case 4: hours = bankaccount.HasCourierDeliveryPosHours; break;
                    case 5: hours = bankaccount.HasCourierDeliveryCashHours; break;
                    case 6: hours = bankaccount.HasFishHours; break;
                    case 7: hours = bankaccount.torob_cancel_minutes / 60.0; break;
                    case 8: hours = bankaccount.dgpay_cancel_minutes / 60.0; break;
                    default:
                        break;
                }



                //order
                Order order = new Order()
                {
                    CurrencyId = 1,
                    DisplaySort = 0,
                    InsertDate = DateTime.Now,
                    IsActive = false,
                    IsExpire = false,
                    LanguageId = languageid,
                    New = true,
                    UserId = userid,
                    Visited = false,
                    BankOrderId = _createOrderKeyService.GetOrderId(),
                    ExpireDate = dateTime.AddHours(hours),
                    AminUserId = adminuserid

                };
                if (!String.IsNullOrEmpty(UserPayAttach))
                    order.UserPayAttach = new Guid(UserPayAttach);
                order.CustomerOrderId = CoreLib.Infrastructure.CommonFunctions.GetOrderCode(order.BankOrderId);
                customid = order.CustomerOrderId;
                #region کد و بن تخفیف
                if (!ProductBasketItems.Any(x => x.hasoff))
                {
                    if (!String.IsNullOrEmpty(codeGift))
                    {
                        bool validUserCodeGift = false;
                        var usercode = context.UserCodeGifts.Where(x => x.UserId == userid && x.Code == codeGift).Include("Offer.offerProductCategories").Include("UserCodeGiftLogs").Include("UserCodeGiftLogs.Order").Select(x => new { x.Id, x.Value, x.CodeType, x.Offer, x.UserCodeGiftLogs, x.Code, x.CountUse, x.IsActive, x.MaxValue, x.Offer.ExpireDate, x.Offer.StartDate, eeExpireDate = x.ExpireDate, x.Offer.offerProductCategories }).SingleOrDefault();
                        if (usercode != null)
                        {
                            UsergiftCodeId = usercode.Id;
                            if (usercode.Offer.IsDeleted == false && usercode.IsActive && usercode.Offer.IsActive && usercode.Offer.state && (usercode.StartDate == null || usercode.StartDate <= dateTime) && (usercode.ExpireDate == null || usercode.ExpireDate >= dateTime) && (usercode.eeExpireDate == null || usercode.eeExpireDate >= dateTime) && (usercode.UserCodeGiftLogs.Where(b => b.state == true).Count() < usercode.CountUse || usercode.CountUse == 0))//کد تخفیف نامعتبر
                            {
                                if (usercode.Offer != null)
                                {
                                    if (usercode.Offer.CodeTypeValueCode == 3)
                                    {
                                        FreeSendPrice = true;
                                        OffPrice = 0;
                                        code_bon = true;
                                    }
                                    else
                                    {
                                        long BasketSum = ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity);
                                        if (usercode.offerProductCategories.Any())
                                        {
                                            List<int> Offercats = usercode.offerProductCategories.Select(a => a.CatId).ToList();
                                            List<int> OffercatsWithSubcats = new List<int>();
                                            foreach (var item in Offercats)
                                                OffercatsWithSubcats.AddRange(context.Database.SqlQuery<int>("exec GetSubCats @CatId", new SqlParameter("@CatId", item)).ToList());
                                            BasketSum = ProductBasketItems.Where(x => OffercatsWithSubcats.Contains(x.CatId)).Sum(x => x.finalPrice * x.UserQuantity);
                                        }

                                        long ofp = Convert.ToInt64(Math.Ceiling(usercode.CodeType == 1 ? usercode.Value : Math.Ceiling(((usercode.Value / 100.0) * BasketSum) * 0.001) * 1000));
                                        if (ofp > usercode.MaxValue && usercode.MaxValue > 0)
                                            ofp = usercode.MaxValue;
                                        OffPrice = ofp;
                                        code_bon = true;
                                    }
                                    validUserCodeGift = true;
                                }
                            }
                        }
                        if (validUserCodeGift == false)
                        {
                            var GeneralCodeGift = context.GeneralCodeGift.Include("Offer.offerProductCategories").Include("GeneralCodeGiftLogs").Include("GeneralCodeGiftLogs.Order").Include("GeneralCodeGiftLists").Where(x => (x.Code == codeGift || x.GeneralCodeGiftLists.Any(b => b.Code == codeGift))).Select(x => new { x.Id, x.Offer, x.IsActive, StartDate = x.Offer.StartDate, x.Offer.ExpireDate, x.generalCode, x.MaxValue, x.OfferId, x.Value, x.CodeType, x.Offer.CodeTypeValueCode, x.GeneralCodeGiftLogs, x.CountUse, x.Offer.offerProductCategories, x.GeneralCodeGiftLists }).FirstOrDefault();
                            if (GeneralCodeGift != null)
                            {
                                giftCodeId = GeneralCodeGift.Id;
                                if (GeneralCodeGift.GeneralCodeGiftLists.Any(s => s.Code == codeGift))
                                    giftListCodeId = GeneralCodeGift.GeneralCodeGiftLists.Where(s => s.Code == codeGift).First().Id;
                                if (GeneralCodeGift.Offer.IsDeleted == false && GeneralCodeGift.IsActive && GeneralCodeGift.Offer.IsActive && GeneralCodeGift.Offer.state && (GeneralCodeGift.StartDate == null || GeneralCodeGift.StartDate <= dateTime) && (GeneralCodeGift.ExpireDate == null || GeneralCodeGift.ExpireDate >= dateTime) && (GeneralCodeGift.GeneralCodeGiftLogs.Where(b => b.state == true && ((b.Order.UserId == userid && b.GeneralCodeGift.CountUseSelect == true) || b.GeneralCodeGift.CountUseSelect == false)).Count() < GeneralCodeGift.CountUse || GeneralCodeGift.CountUse == 0))//کد تخفیف نامعتبر
                                {
                                    if (GeneralCodeGift.generalCode == GeneralCodeType.تخفیف_ارسال_رایگان)
                                    {

                                        FreeSendPrice = true;
                                        OffPrice = 0;
                                        code_bon = true;
                                    }
                                    else
                                    {
                                        long BasketSum = ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity);
                                        if (GeneralCodeGift.offerProductCategories.Any())
                                        {
                                            List<int> Offercats = GeneralCodeGift.offerProductCategories.Select(a => a.CatId).ToList();
                                            List<int> OffercatsWithSubcats = new List<int>();
                                            foreach (var item in Offercats)
                                                OffercatsWithSubcats.AddRange(context.Database.SqlQuery<int>("exec GetSubCats @CatId", new SqlParameter("@CatId", item)).ToList());
                                            BasketSum = ProductBasketItems.Where(x => OffercatsWithSubcats.Contains(x.CatId)).Sum(x => x.finalPrice * x.UserQuantity);
                                        }

                                        long ofp = Convert.ToInt64(Math.Ceiling(GeneralCodeGift.CodeType == 1 ? GeneralCodeGift.Value : Math.Ceiling(((GeneralCodeGift.Value / 100.0) * BasketSum) * 0.001) * 1000));
                                        if (ofp > GeneralCodeGift.MaxValue && GeneralCodeGift.MaxValue > 0)
                                            ofp = GeneralCodeGift.MaxValue;
                                        OffPrice = ofp;
                                        code_bon = true;
                                    }

                                }
                            }
                        }
                    }
                    else if (bon > 0)
                    {
                        int UserCurrentBon = context.UserBons.Where(x => x.UserId == userid && x.state == true && x.ExpireDate > dateTime && x.Value > x.UsedValue).Sum(x => x.Value - x.UsedValue);
                        int? UserMaxBon = ProductBasketItems.Sum(x => x.MaxBon);

                        if (UserCurrentBon > 0 && bon <= UserCurrentBon && (bon <= UserMaxBon || UserMaxBon == 0))//بن تخفیف نامعتبر
                        {
                            code_bon = false;
                            OffPrice = setting.BonPrice * bon;
                        }
                    }
                }
                #endregion



                order.OrderDeliveries = new List<OrderDelivery>();
                try
                {

                    //order delivery
                    if (BasketItems.First().MergeId.HasValue)//merge
                    {
                        var delivery = context.OrderDeliveries.Find(BasketItems.First().MergeId);
                        order.OrderDeliveries.Add(new OrderDelivery()
                        {
                            DeliveryState = DeliveryState.تحویل_به_موقع,
                            ProductSendWayWorkTimeId = delivery.ProductSendWayWorkTimeId,
                            RequestDate = delivery.RequestDate,
                            PredictDate = delivery.PredictDate,
                            SendWayId = delivery.SendWayId,
                            UserAddressId = delivery.UserAddressId,
                            Address = delivery.Address,
                            FooterGoogleMapLatitude = delivery.FooterGoogleMapLatitude,
                            FooterGoogleMapLongitude = delivery.FooterGoogleMapLongitude,
                            City = delivery.City,
                            Provience = delivery.Provience,
                            PostalCode = delivery.PostalCode,
                            Mobile = delivery.Mobile,
                            FullName = delivery.FullName,
                            LandlinePhone = delivery.LandlinePhone,
                            ProductPackageType = delivery.ProductPackageType,
                            SendwayBoxId = delivery.SendwayBoxId,
                            Insurance = delivery.Insurance,
                            DeliveryPos = delivery.DeliveryPos
                        });
                        order.OrderId = delivery.OrderId;
                    }
                    else//new delivery
                    {
                        foreach (var item in BasketItems.First().shippings)
                        {

                            int? adid = null, wid = null;
                            if (item.addressId > 0)
                                adid = item.addressId;
                            if (item.sendwayWorktimeId > 0)
                                wid = item.sendwayWorktimeId;

                            DateTime? reqdate = null;
                            DateTime? predate = null;
                            if (item.deliveryDate != null)
                            {

                                reqdate = predate = Convert.ToDateTime(item.deliveryDate);
                                var timeselect = context.ProductSendWayWorkTimes.Find(wid.Value).EndTime;
                                if (PaymentType == 4)
                                {
                                    hours = bankaccount.HasCourierDeliveryPosHours;
                                    order.ExpireDate = reqdate.Value.AddHours(hours).AddHours(timeselect.TotalHours);
                                }
                                else if (PaymentType == 5)
                                {
                                    hours = bankaccount.HasCourierDeliveryCashHours;
                                    order.ExpireDate = reqdate.Value.AddHours(hours).AddHours(timeselect.TotalHours);
                                }

                            }
                            else
                            {

                                var sendway = context.ProductSendWays.Find(item.sendwayId);
                                predate = DateTime.Now.AddDays(ProductBasketItems.Max(x => x.DeliveryTimeout)).AddDays(sendway.DeliveryHourDay);
                                for (int i = 1; i < ProductBasketItems.Max(x => x.DeliveryTimeout) + sendway.DeliveryHourDay; i++)
                                {
                                    DateTime T1 = DateTime.Now.AddDays(i);
                                    if (context.Holidais.Where(x => x.InsertDate == T1.Date).Any() || T1.Date.DayOfWeek == DayOfWeek.Friday)
                                        predate = predate.Value.AddDays(1);
                                }
                                var t2 = predate.Value.Date;
                                while (context.Holidais.Where(x => x.InsertDate == t2).Any() || t2.DayOfWeek == DayOfWeek.Friday)
                                {
                                    t2 = t2.AddDays(1);
                                }
                                predate = t2;

                            }

                            var useraddress = getUserAddress(adid, cuser);
                            if (item.sellerId == 0)
                                item.sellerId = context.Sellers.First().Id;

                            order.OrderDeliveries.Add(new OrderDelivery()
                            {
                                DeliveryState = DeliveryState.تحویل_به_موقع,
                                ProductSendWayWorkTimeId = wid,
                                RequestDate = reqdate,
                                PredictDate = predate,
                                SendWayId = item.sendwayId,
                                UserAddressId = adid,
                                Address = useraddress.address,
                                FullName = useraddress.fullname,
                                PostalCode = useraddress.postalcode,
                                Mobile = useraddress.mobile,
                                Provience = useraddress.provience,
                                City = useraddress.city,
                                nationalCode = useraddress.nationalCode,
                                LandlinePhone = useraddress.LandlinePhone,
                                ProductPackageType = item.ProductPackageType,
                                SendwayBoxId = item.sendwayBoxId,
                                Insurance = item.extraprice,
                                DeliveryPos = item.extrapricePos,
                                FooterGoogleMapLatitude = useraddress.FooterGoogleMapLatitude,
                                FooterGoogleMapLongitude = useraddress.FooterGoogleMapLongitude,
                                SellerId = item.sellerId

                            });


                        }
                    }



                    Insert(order);
                    context.SaveChanges();

                    _createOrderKeyService.updateGetOrderId(order.BankOrderId, order.Id);

                    //order row
                    order.OrderRows = new List<OrderRow>();
                    foreach (var item in ProductBasketItems.GroupBy(x => x.PacakgeType))
                    {
                        foreach (var item2 in ProductBasketItems.Where(x => x.PacakgeType == item.Key))
                        {
                            order.OrderRows.Add(new OrderRow()
                            {
                                Price = item2.finalPrice,
                                ProductId = item2.ProductId,
                                ProductPriceId = item2.Id,
                                Quantity = item2.UserQuantity,
                                RawPrice = item2.Price,
                                ProductOfferId = item2.offerid,
                                OrderDeliveryId = order.OrderDeliveries.Where(x => x.ProductPackageType == item2.ProductPackageType).First().Id,
                                taxValue = UserIsHaghighi ? Convert.ToInt32(Math.Ceiling(item2.finalPrice * item2.UserQuantity * (context.Taxes.Find(item2.TaxId).TaxPercent / 100.0))) : 0,
                                Profit = Convert.ToInt64(Math.Ceiling(item2.finalPrice * (context.ProductPrices.Find(item2.Id).Profit / 100.0)))
                            });
                            var pr = context.Products.Find(item2.ProductId);
                            pr.SellCount += item2.UserQuantity;
                        }
                    }
                    Update(order);
                    context.SaveChanges();

                    //order attributes
                    order.OrderAttributeSelects = new List<OrderAttributeOrder>();
                    order.OrderStates = new List<OrderState>();
                    OrderAttribute sendPrice = context.OrderAttributes.Where(x => x.DataType == 14).Single();
                    OrderAttribute sendPackagePrice = context.OrderAttributes.Where(x => x.DataType == 31).Single();
                    OrderAttribute sendPaskeraye = context.OrderAttributes.Where(x => x.DataType == 32).Single();
                    OrderAttribute sendrawPrice = context.OrderAttributes.Where(x => x.DataType == 33).Single();

                    OrderAttribute valueAdded = context.OrderAttributes.Where(x => x.DataType == 15).Single();
                    OrderAttribute usrdescr = context.OrderAttributes.Where(x => x.DataType == 18).Single();
                    OrderAttribute sendfactor = context.OrderAttributes.Where(x => x.DataType == 25).Single();
                    OrderAttribute priceOff = context.OrderAttributes.Where(x => x.DataType == 23).Single();
                    OrderAttribute priceOffType = context.OrderAttributes.Where(x => x.DataType == 24).Single();
                    OrderAttribute feeSend = context.OrderAttributes.Where(x => x.DataType == 29).Single();
                    long sumPrice = 0, sumOff = 0;
                    int cost = 0, sumcost = 0, sendprice = 0, sumsendprice = 0;
                    int commision = 0, sumcommision = 0;
                    foreach (var item in context.OrderDeliveries.Where(x => x.OrderId == order.Id))
                    {
                        int CityId = item.UserAddressId > 0 ? context.UserAddresses.Find(item.UserAddressId.Value).CityId.Value : context.Users.Find(userid).CityId.Value;
                        sumOff = 0;


                        //هزینه ارسال
                        if (FreeSendPrice)
                        {
                            cost = 0;
                            sendprice = 0;
                        }
                        else
                        {
                            int extraPrice = ProductBasketItems.Max(x => x.extraprice);
                            int InsuranceCost = 0;
                            int? costvalue = 0;
                            int PackageCost = 0;
                            int DeliveryPosCost = 0;
                            int Extraprice2Cost = 0;
                            if (BasketItems.First().MergeId == null)
                            {
                                InsuranceCost = (item.ProductSendWay.HasExtraPrice && BasketItems.First().shippings.Where(x => x.sendwayId == item.ProductSendWay.Id).First().extraprice ? Convert.ToInt32(Math.Ceiling((ProductBasketItems.Sum(x => x.finalPrice) * 0.005) * 0.001) * 1000) : 0);

                                InsuranceCost = InsuranceCost > 0 ? InsuranceCost > 100000 ? 100000 : InsuranceCost < 15000 ? 15000 : InsuranceCost : 0;
                                //PackageCost = (item.ProductSendWay.HasExtraPrice ? Convert.ToInt32(Math.Ceiling(extraPrice * 0.001) * 1000) : 0);
                                PackageCost = (item.ProductSendWay.HasExtraPrice ? Convert.ToInt32(Math.Ceiling((extraPrice + context.SendwayBoxes.Find(item.SendwayBox.Id).BoxPrice) * 0.001) * 1000) : 0);

                                DeliveryPosCost = item.DeliveryPos ? Convert.ToInt32(bankaccount.DeliveryPosPrice) : 0;
                                Extraprice2Cost = item.ProductSendWay.HasExtraPrice2 ? item.ProductSendWay.ExtraPriceValue : 0;

                                costvalue = _productPriceService.GetSendwayCost(ProductBasketItems.Select(x => x.Id).ToList(), CityId, BasketItems.First().shippings.Where(x => x.ProductPackageType == item.ProductPackageType).First().sendwayBoxId, item.SendWayId.Value, ProductBasketItems.Sum(x => x.productWeight * x.UserQuantity), ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity), PackageCost, out FreeSendPrice) + InsuranceCost + PackageCost + DeliveryPosCost + Extraprice2Cost;
                                if (costvalue == -1)
                                {
                                    validFreeSend = false;
                                    return order.BankOrderId;
                                }
                                else if (FreeSendPrice)
                                {
                                    cost = 0;
                                    sendprice = 0;
                                }
                                else
                                {
                                    var SendWayBoxPrice = productBoxes.Where(x => x.SendwayBox.ProductPackageType == item.ProductPackageType).SelectMany(x => x.sendWayBoxPrices.Where(a => a.productSendWay.Id == item.SendWayId)).OrderByDescending(x => x.BoxMass).ThenByDescending(s => s.IsDefault).ThenBy(s => s.PasKeraye).ThenBy(s => s.cost).First();
                                    cost = SendWayBoxPrice.PasKeraye ? (0 + Extraprice2Cost + PackageCost) : costvalue.HasValue ? costvalue.Value : 0;
                                    sendprice = SendWayBoxPrice.PasKeraye ? 0 : costvalue.HasValue ? costvalue.Value : 0;

                                    if (FreeSendPrice)
                                    {
                                        cost = 0;
                                        sendprice = 0;
                                    }
                                    //هزینه بسته بندی
                                    order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                                    {
                                        AttributeId = sendPackagePrice.Id,
                                        OrderDeliveryId = item.Id,
                                        Value = SendWayBoxPrice.PasKeraye ? (0 + Extraprice2Cost + PackageCost).ToString() : "0"
                                    });
                                    order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                                    {
                                        AttributeId = sendPaskeraye.Id,
                                        OrderDeliveryId = item.Id,
                                        Value = SendWayBoxPrice.PasKeraye ? "1" : "0"
                                    });

                                    //else if (!SendWayBoxPrice.PasKeraye)
                                    //    cost = SendWayBoxPrice.cost;
                                }
                            }
                        }
                        sumcost += cost;
                        if (BasketItems.First().MergeId.HasValue)
                        {
                            cost = 0;
                            sendprice = 0;
                            sumcost = 0;
                        }
                        order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                        {
                            AttributeId = sendPrice.Id,
                            OrderDeliveryId = item.Id,
                            Value = cost.ToString()
                        });
                        order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                        {
                            AttributeId = sendrawPrice.Id,
                            OrderDeliveryId = item.Id,
                            Value = sendprice.ToString()
                        });

                        //جمع محصولات
                        List<int> ids = productBoxes.Where(x => x.SendwayBox.ProductPackageType == item.ProductPackageType).SelectMany(x => x.ProductPriceIdList).ToList();
                        int valueAddedval = 0;
                        foreach (var product in ProductBasketItems.Where(x => x.ProductPackageType == item.ProductPackageType))
                        {

                            if (product.TaxId.HasValue && UserIsHaghighi)
                                valueAddedval += Convert.ToInt32(Math.Ceiling(product.finalPrice * product.UserQuantity * (context.Taxes.Find(product.TaxId).TaxPercent / 100.0)));
                            sumPrice += (product.finalPrice * product.UserQuantity) + valueAddedval;
                            sumOff += product.offFinalValue * product.UserQuantity;
                        }

                        if (OffPrice == -1)
                        {
                            if (UsergiftCodeId.HasValue || giftCodeId.HasValue)
                            {
                                order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                                {
                                    AttributeId = priceOffType.Id,
                                    OrderDeliveryId = item.Id,
                                    Value = "4"
                                });
                            }

                            //مقدار تخفیف محصول 
                            order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                            {
                                AttributeId = priceOff.Id,
                                OrderDeliveryId = item.Id,
                                Value = sumOff.ToString()
                            });
                        }
                        //ارزش افزوده
                        order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                        {
                            AttributeId = valueAdded.Id,
                            OrderDeliveryId = item.Id,
                            Value = valueAddedval.ToString()
                        });
                        //توضیحات کاربر
                        order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                        {
                            AttributeId = usrdescr.Id,
                            OrderDeliveryId = item.Id,
                            Value = !String.IsNullOrEmpty(userDescription) ? userDescription : "-"
                        });
                        //ارسال فاکتور چاپی
                        order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                        {
                            AttributeId = sendfactor.Id,
                            OrderDeliveryId = item.Id,
                            Value = sendFactor ? "1" : "0"
                        });
                        //وضعیت
                        order.OrderStates.Add(new OrderState()
                        {
                            LogDate = DateTime.Now,
                            OrderDeliveryId = item.Id,
                            state = OrderStatus.در_انتظار_تایید
                        });
                        List<int> productPriceId = order.OrderRows.Select(x => x.ProductPriceId.Value).ToList();
                        if (!context.ProductPrices.Any(x => productPriceId.Contains(x.Id) && x.ProductStateId == 2))
                        {
                            order.OrderStates.Add(new OrderState()
                            {
                                LogDate = DateTime.Now,
                                OrderDeliveryId = item.Id,
                                state = OrderStatus.تایید_سفارش
                            });
                        }
                        else
                            IsEstelam = true;

                        //off
                        if (OffPrice >= 0)
                        {
                            order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                            {
                                AttributeId = priceOffType.Id,
                                OrderDeliveryId = item.Id,
                                Value = code_bon.Value ? "4" : "3"
                            });
                            order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                            {
                                AttributeId = priceOff.Id,
                                OrderDeliveryId = item.Id,
                                Value = OffPrice.ToString()
                            });

                        }
                        //ارسال رایگان
                        if (FreeSendPrice)
                        {
                            order.OrderAttributeSelects.Add(new OrderAttributeOrder()
                            {
                                AttributeId = feeSend.Id,
                                OrderDeliveryId = item.Id,
                                Value = "1"
                            });
                        }
                    }


                    Update(order);
                    context.SaveChanges();

                    //code and bon log
                    if (!String.IsNullOrEmpty(codeGift) && UsergiftCodeId.HasValue && code_bon == true)
                    {
                        order.UserCodeGiftLogs = new List<UserCodeGiftLog>();
                        order.UserCodeGiftLogs.Add(new UserCodeGiftLog()
                        {
                            InsertDate = dateTime,
                            UserCodeGiftId = UsergiftCodeId.Value,
                            Value = OffPrice,
                            state = false
                        });
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(codeGift) && giftCodeId.HasValue && code_bon == true)
                        {
                            order.GeneralCodeGiftLogs = new List<GeneralCodeGiftLog>();
                            order.GeneralCodeGiftLogs.Add(new GeneralCodeGiftLog()
                            {
                                InsertDate = dateTime,
                                GeneralCodeGiftId = giftCodeId.Value,
                                Value = OffPrice,
                                state = false,
                                GeneralCodeGiftListId = giftListCodeId
                            });
                        }
                    }
                    if (bon > 0 && code_bon == false && OffPrice > 0)
                    {
                        var tempbon = bon;
                        order.UserBonLogs = new List<UserBonLog>();
                        foreach (var item2 in context.UserBons.Where(x => x.UserId == userid && x.state == true && x.ExpireDate > dateTime && x.Value > x.UsedValue).OrderBy(x => x.ExpireDate))
                        {
                            if ((item2.Value - item2.UsedValue) > 0)
                            {
                                order.UserBonLogs.Add(new UserBonLog()
                                {
                                    InsertDate = dateTime,
                                    UserBonId = item2.Id,
                                    Value = OffPrice,
                                    state = false
                                });

                                if ((item2.Value - item2.UsedValue) >= tempbon)
                                {
                                    item2.UsedValue += tempbon;
                                    tempbon = 0;
                                    break;
                                }
                                else
                                {
                                    item2.UsedValue += (item2.Value - item2.UsedValue);
                                    tempbon = (item2.Value - item2.UsedValue);
                                }
                            }
                        }
                    }
                    if (ProductBasketItems.Sum(x => x.Bon) > 0)
                    {
                        order.UserBons = new List<UserBon>();
                        order.UserBons.Add(new UserBon()
                        {
                            InsertDate = dateTime,
                            state = false,
                            UsedValue = 0,
                            Value = ProductBasketItems.Sum(x => x.Bon).Value,
                            UserId = userid,
                            ExpireDate = dateTime.AddDays(setting.BonExpireDay)
                        });
                    }



                    //commision 
                    foreach (var item in context.OrderDeliveries.Where(x => x.OrderId == order.Id))
                    {
                        switch (PaymentType)
                        {
                            case 1:
                            case 2:
                                if (bankaccount.Commision > 0)
                                {
                                    commision = Convert.ToInt32(Math.Ceiling((sumPrice + sumcost - (OffPrice > 0 ? OffPrice : 0)) * (bankaccount.Commision / 100.0)));
                                    commision = Convert.ToInt32(Math.Ceiling(commision * 0.001) * 1000);
                                    sumcommision += commision;
                                }
                                break;
                            case 3:
                                if (bankaccount.Commisioncard > 0)
                                {
                                    commision = Convert.ToInt32(Math.Ceiling((sumPrice + sumcost - (OffPrice > 0 ? OffPrice : 0)) * (bankaccount.Commisioncard / 100.0)));
                                    commision = Convert.ToInt32(Math.Ceiling(commision * 0.001) * 1000);
                                    sumcommision += commision;
                                }
                                break;
                            case 7:
                                if (bankaccount.Commisiontorob > 0)
                                {
                                    commision = Convert.ToInt32(Math.Ceiling((sumPrice + sumcost - (OffPrice > 0 ? OffPrice : 0)) * (bankaccount.Commisiontorob / 100.0)));
                                    commision = Convert.ToInt32(Math.Ceiling(commision * 0.001) * 1000);
                                    sumcommision += commision;
                                }
                                break;
                            case 8:
                                if (bankaccount.Commisiondgpay > 0)
                                {
                                    commision = Convert.ToInt32(Math.Ceiling((sumPrice + sumcost - (OffPrice > 0 ? OffPrice : 0)) * (bankaccount.Commisiondgpay / 100.0)));
                                    commision = Convert.ToInt32(Math.Ceiling(commision * 0.001) * 1000);
                                    sumcommision += commision;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    order.Commision = sumcommision;

                    Update(order);
                    context.SaveChanges();


                    //wallet
                    Wallet NewWallet = new Wallet();
                    NewWallet.BankAccountId = BankAccountId;
                    NewWallet.DepositOrWithdrawal = true;
                    NewWallet.ForWhat = context.ForWhats.Where(x => x.ForWhatType == ForWhatType.پرداخت_سفارش).SingleOrDefault();
                    NewWallet.InsertDate = DateTime.Now;
                    NewWallet.LanguageId = languageid;
                    NewWallet.PaymentType = PaymentType;
                    NewWallet.Price = sumPrice + sumcost - (OffPrice > 0 ? OffPrice : 0) + sumcommision;
                    NewWallet.State = false;
                    NewWallet.UserId = order.UserId;
                    if (IsEstelam)
                    {
                        NewWallet.WalletAttributeWallets = new List<WalletAttributeWallet>();
                        foreach (var item in context.OrderDeliveries.Where(x => x.OrderId == order.Id))
                        {

                            NewWallet.WalletAttributeWallets.Add(new WalletAttributeWallet()
                            {
                                WalletAttributeId = context.WalletAttributes.Where(x => x.DataType == 23).Single().Id,
                                Value = "0",
                                OrderDeliveryId = item.Id
                            });

                        }

                        //if now shopping is off
                        var dateTimeday = DateTime.Now.TimeOfDay;
                        short weekday = (short)DateTime.Now.PersionDayOfWeek();
                        if (!context.ShoppingWorkTimes.Where(x => x.SettingId == setting.Id && x.IsActive && x.WeekDay == weekday && x.StartTime < dateTimeday && x.EndTime > dateTimeday).Any())
                        {
                            var nextDate = context.ShoppingWorkTimes.Where(x => x.SettingId == setting.Id && x.IsActive && x.WeekDay > weekday).OrderBy(x => x.WeekDay).ThenBy(x => x.StartTime).FirstOrDefault();
                            if (nextDate == null)
                                nextDate = context.ShoppingWorkTimes.OrderBy(x => x.WeekDay).ThenBy(x => x.StartTime).FirstOrDefault();
                            var finalDate = DateTime.Now.Date;
                            for (int i = 1; i < 7; i++)
                            {
                                if ((short)finalDate.AddDays(i).PersionDayOfWeek() == nextDate.WeekDay)
                                    finalDate = finalDate.AddDays(i).AddHours(nextDate.StartTime.TotalHours).AddHours(hours);

                            }
                            order.ExpireDate = finalDate;
                        }

                    }
                    context.Wallets.Add(NewWallet);
                    context.SaveChanges();
                    //walletorder
                    order.OrderWallets = new List<OrderWallet>();
                    order.OrderWallets.Add(new OrderWallet() { Wallet = NewWallet });
                    Update(order);
                    context.SaveChanges();



                    //codegiftlog
                    //bon gift
                    //bongiftlog

                    //order isactive and new set true



                    return order.BankOrderId;
                }
                catch (Exception ex)
                {
                    try
                    {

                        Delete(order);
                        context.SaveChanges();
                    }
                    catch (Exception)
                    {

                    }

                    Domain.EventLog eventLog = new EventLog()
                    {
                        IP = "127.0.0.1",
                        LogType = 5,
                        ControllerName = "Cart",
                        ActionName = "AddOrder",
                        RequestType = false,
                        StatusCode = 500,
                        Description = ex.Message + ex.InnerException != null ? ex.InnerException.Message : "",
                        LogDateTime = DateTime.Now,
                        UserId = userid
                    };
                    context.EventLogs.Add(eventLog);
                    context.SaveChanges();

                    return 0;
                }
            }
            catch (Exception ex)
            {
                IsEstelam = false;
                validFreeSend = true;
                EventLog eventLog = new EventLog()
                {
                    IP = "127.0.0.1",
                    LogType = 5,
                    ControllerName = "Cart",
                    ActionName = "AddOrder",
                    RequestType = false,
                    StatusCode = 500,
                    Description = ex.Message + ex.InnerException != null ? ex.InnerException.Message : "",
                    LogDateTime = DateTime.Now,
                    UserId = userid
                };
                context.EventLogs.Add(eventLog);
                context.SaveChanges();
                customid = ex.Message;
                return 0;
            }
        }

        //public async Task<bool> UpdateQuantity(Order order)
        //{
        //    try
        //    {
        //        //update quantity
        //        foreach (var item in order.OrderRows)
        //        {
        //            var prp = context.ProductPrices.Find(item.ProductPriceId);
        //            prp.Quantity -= item.Quantity;
        //            //prp.MaxBasketCount -= item.Quantity;
        //            //ناموجود شود
        //            if (prp.Quantity < 1)
        //            {
        //                prp.Quantity = 0;
        //                prp.ProductStateId = 5;
        //                ////اگر تنوع پیش فرض می باشد، محصول ناموجود شود
        //                //if (prp.IsDefault)
        //                //{
        //                //    var pr = context.Products.Find(item.ProductId);
        //                //    pr.ProductStateId = 5;
        //                //}
        //            }
        //            //if (prp.MaxBasketCount < 1)
        //            //    prp.MaxBasketCount = 0;
        //            await context.SaveChangesAsync();

        //            var proffer = context.ProductOffers.Where(a => a.ProductPrice.ProductStateId < 3 && a.ProductPriceId == item.ProductPriceId && a.Offer.IsActive && a.Offer.state == true && ((a.Offer.ExpireDate != null && a.Offer.ExpireDate >= DateTime.Now) || a.Offer.ExpireDate == null) && ((a.Offer.StartDate != null && a.Offer.StartDate <= DateTime.Now) || a.Offer.StartDate == null)).FirstOrDefault();
        //            if (proffer != null)
        //            {
        //                proffer.Quantity -= item.Quantity;
        //                //proffer.MaxBasketCount -= item.Quantity;
        //                if (proffer.Quantity < 1)
        //                    proffer.Quantity = 0;
        //                //if (proffer.MaxBasketCount < 1)
        //                //    proffer.MaxBasketCount = 0;
        //            }
        //            await context.SaveChangesAsync();
        //        }
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        //public bool CheckQuantity(Order order)
        //{
        //    try
        //    {
        //        //update quantity
        //        foreach (var item in order.OrderRows)
        //        {
        //            var prp = context.ProductPrices.Find(item.ProductPriceId);
        //            prp.Quantity += item.Quantity;
        //            //موچود شود
        //            if (prp.Quantity > 0 && prp.ProductStateId == 5)
        //            {
        //                prp.ProductStateId = 1;
        //                ////اگر تنوع پیش فرض می باشد، محصول موجود شود
        //                //if (prp.IsDefault)
        //                //{
        //                //    var pr = context.Products.Find(item.ProductId);
        //                //    pr.ProductStateId = 1;
        //                //    context.SaveChanges();
        //                //}
        //            }
        //            _productPriceService.Update(prp);
        //            context.SaveChanges();

        //            var proffer = context.ProductOffers.Where(a => a.ProductPrice.ProductStateId < 3 && a.ProductPriceId == item.ProductPriceId && a.Offer.IsActive && a.Offer.state == true && ((a.Offer.ExpireDate != null && a.Offer.ExpireDate >= DateTime.Now) || a.Offer.ExpireDate == null) && ((a.Offer.StartDate != null && a.Offer.StartDate <= DateTime.Now) || a.Offer.StartDate == null)).FirstOrDefault();
        //            if (proffer != null)
        //            {
        //                proffer.Quantity += item.Quantity;
        //                context.SaveChanges();
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        public bool GiftCatValid(List<int> giftCatIds, List<int> basketCatIds)
        {
            bool valid = false;
            if (giftCatIds.Any())
            {
                foreach (var item in basketCatIds)
                {
                    if (giftCatIds.Any(x => x == item))
                        valid = true;
                }
            }
            return valid;
        }

        public Domain.ViewModels.userAdr getUserAddress(int? adid, ApplicationUser user)
        {
            userAdr userAdr = new userAdr();
            if (adid.HasValue)
            {
                var useraddress = user.UserAddresses.Where(x => x.Id == adid.Value).First();
                userAdr.address += string.Format("{0} {1} {2}", useraddress.Address, !String.IsNullOrEmpty(useraddress.AddressNumber) ? " پلاک " + useraddress.AddressNumber : "", !String.IsNullOrEmpty(useraddress.AddressUnit) ? " ، واحد " + useraddress.AddressUnit : "");
                userAdr.fullname = useraddress.FullName;
                userAdr.mobile = useraddress.PhoneNumber;
                userAdr.postalcode = useraddress.PostalCode;
                userAdr.city = useraddress.CityEntity.Name;
                userAdr.provience = useraddress.CityEntity.Province.Name;
                userAdr.LandlinePhone = useraddress.LandlinePhone;
                userAdr.nationalCode = user.NationalCode;
                userAdr.FooterGoogleMapLatitude = useraddress.FooterGoogleMapLatitude;
                userAdr.FooterGoogleMapLongitude = useraddress.FooterGoogleMapLongitude;
            }
            else
            {
                userAdr.address += string.Format("{0} {1} {2}", user.Address, !String.IsNullOrEmpty(user.AddressNumber) ? " پلاک " + user.AddressNumber : "", !String.IsNullOrEmpty(user.AddressUnit) ? " ، واحد " + user.AddressUnit : "");
                userAdr.fullname = user.FirstName + " " + user.LastName;
                userAdr.mobile = user.PhoneNumber;
                userAdr.postalcode = user.PostalCode;
                userAdr.city = user.CityEntity.Name;
                userAdr.provience = user.CityEntity.Province.Name;
                userAdr.LandlinePhone = user.LandlinePhone;
                userAdr.nationalCode = user.NationalCode;
                userAdr.FooterGoogleMapLatitude = user.FooterGoogleMapLatitude;
                userAdr.FooterGoogleMapLongitude = user.FooterGoogleMapLongitude;
            }
            return userAdr;
        }

        #region OrderV2 pagination helpers
        // این چند تا متد برای رفع "Execution Timeout Expired" روی کاربرهایی با تعداد سفارش زیاد اضافه شدن.
        // قبلاً هر ۴ متد GetXxxOrderV2 کل تاریخچه‌ی سفارشات کاربر رو (با Include های سنگین و چندلایه)
        // می‌آوردن تو حافظه و بعد صفحه‌بندی روی همون لیست کامل انجام می‌شد (ToPagedList روی List).
        // الان صفحه‌بندی (Skip/Take) قبل از Include سنگین و توی خود SQL انجام می‌شه: اول فقط Id سفارش‌های
        // همون صفحه گرفته می‌شه (کوئری سبک)، بعد فقط همون چندتا سفارش hydrate می‌شن.

        private IQueryable<Order> ApplyOrderFilters(IQueryable<Order> orders, string date, string keyword, int? filterState = null)
        {
            if (!String.IsNullOrEmpty(date))
            {
                DateTime dt = DateTime.Now.AddDays(-31);
                if (date == "2")
                    dt = DateTime.Now.AddDays(-90);
                else if (date == "3")
                    dt = DateTime.Now.AddDays(-365);
                orders = orders.Where(x => x.InsertDate >= dt);
            }
            if (!String.IsNullOrEmpty(keyword))
            {
                var color = context.ProductAttributeItemColors.Where(x => x.Color.Contains(keyword)).FirstOrDefault();
                string colorId = "0";
                if (color != null)
                    colorId = color.Id.ToString();
                var Garanty = context.ProductAttributeItems.Where(x => x.Value.Contains(keyword)).FirstOrDefault();
                string GarantyId = "0";
                if (Garanty != null)
                    GarantyId = Garanty.Id.ToString();
                orders = from s in orders
                         where
                         (s.CustomerOrderId == keyword) ||
                                (s.OrderRows.Any(x => x.Product.LatinName != null) && s.OrderRows.Any(x => x.Product.LatinName.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.Product.Name != null) && s.OrderRows.Any(x => x.Product.Name.ToLower().Contains(keyword))) ||
                                 (s.OrderRows.Any(x => x.ProductPrice.code != null) && s.OrderRows.Any(x => x.ProductPrice.code.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModelId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectModel.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSizeId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectSize.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectWeightId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectweight.Value.ToLower().Contains(keyword))) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColorId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectColor.Value.ToLower() == colorId)) ||
                                (s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGarantyId != null) && s.OrderRows.Any(x => x.ProductPrice.ProductAttributeSelectGaranty.Value.ToLower() == GarantyId))
                         select s;
            }
            // فیلترِ «وضعیت سفارش» توی صفحه‌ی سفارشات من - مستقل از تبِ فعال (جاری/بسته‌شده/
            // پرداخت‌نشده)؛ روی همون آخرین state هرسفارش (MAX per OrderId) چک می‌شه، عیناً همون
            // الگویی که تب‌بندی هم ازش استفاده می‌کنه. اگه با تبِ فعال هم‌خوانی نداشته باشه (مثلاً
            // تبِ «جاری» + وضعیتِ «لغو شده») نتیجه خالی می‌شه - رفتارِ درست و مورد‌انتظاره.
            if (filterState.HasValue)
            {
                var stateOrders = from x in _orderStateService.GetQueryList().AsNoTracking()
                                  group x by x.OrderId into g
                                  select new { g.Key, state = g.Max(x => x.state) };
                IQueryable<Guid> matchingIds = stateOrders.Where(x => (int)x.state == filterState.Value).Select(x => x.Key);
                orders = orders.Where(x => matchingIds.Contains(x.Id));
            }
            return orders;
        }

        private PagedList.IPagedList<OrderV2> PageAndHydrateOrders(IQueryable<Order> baseQuery, int pageNumber, int pageSize, string sort = null)
        {
            int totalCount = baseQuery.Count();

            // ترتیب: جدیدترین (پیش‌فرض) / قدیمی‌ترین بر مبنای BankOrderId (همون معیارِ قبلی)،
            // گران‌ترین/ارزان‌ترین بر مبنای قیمتِ کیفِ پولِ سفارش (همون فیلدی که در ادامه توی
            // MapOrder به‌عنوانِ Price برمی‌گرده).
            IQueryable<Order> sortedQuery;
            switch (sort)
            {
                case "oldest":
                    sortedQuery = baseQuery.OrderBy(x => x.BankOrderId);
                    break;
                case "expensive":
                    sortedQuery = baseQuery.OrderByDescending(x => x.OrderWallets.Select(w => w.Wallet.Price).FirstOrDefault());
                    break;
                case "cheap":
                    sortedQuery = baseQuery.OrderBy(x => x.OrderWallets.Select(w => w.Wallet.Price).FirstOrDefault());
                    break;
                default:
                    sortedQuery = baseQuery.OrderByDescending(x => x.BankOrderId);
                    break;
            }

            var pageIds = sortedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.Id)
                .ToList();

            if (pageIds.Count == 0)
            {
                return new PagedList.StaticPagedList<OrderV2>(new List<OrderV2>(), pageNumber, pageSize, totalCount);
            }

            var hydrated = GetQueryList().AsNoTracking()
                .Include("OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image")
                .Include("OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel")
                .Include("OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize")
                .Include("OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor")
                .Include("OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute")
                .Include("OrderWallets.Wallet.WalletAttributeWallets.WalletAttribute")
                .Include("OrderStates")
                .Include("OrderDeliveries")
                .Include("OrderDeliveries.ProductSendWay")
                .Include("OrderDeliveries.AdminSelectedSendway")
                .Include("OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image")
                .Include("OrderWallets.Wallet.BankAccount")
                .Include("OrderRates")
                .Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductImages.Image")
                .Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectModel")
                .Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize")
                .Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectColor")
                .Include("ChildOrders.OrderDeliveries.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute")
                .Include("ChildOrders.OrderDeliveries")
                .Include("ChildOrders.OrderDeliveries.ProductSendWay")
                .Include("ChildOrders.OrderDeliveries.AdminSelectedSendway")
                .Include("ChildOrders.OrderDeliveries.OrderRows.Product.ProductPrices.ProductImages.Image")
                .Include("ChildOrders.OrderRows.ProductPrice.ProductImages.Image")
                .Include("ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectModel")
                .Include("ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize")
                .Include("ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectColor")
                .Include("ChildOrders.OrderRows.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute")
                .Include("ChildOrders.OrderRows.Product.ProductPrices.ProductImages.Image")
                .Include("OrderDeliveries.WalletAttributeWallets.WalletAttribute")
                .Where(x => pageIds.Contains(x.Id))
                .ToList();

            var colorlist = context.ProductAttributeItemColors.ToList();

            var mapped = pageIds
                .Select(id => hydrated.FirstOrDefault(o => o.Id == id))
                .Where(o => o != null)
                .Select(x => MapOrder(x, colorlist))
                .ToList();

            return new PagedList.StaticPagedList<OrderV2>(mapped, pageNumber, pageSize, totalCount);
        }

        private OrderV2 MapOrder(Order x, List<ProductAttributeItemColor> colorlist)
        {
            return new OrderV2
            {
                Id = x.Id,
                BankOrderId = x.BankOrderId,
                CustomerOrderId = x.CustomerOrderId,
                InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(x.InsertDate),
                BankId = x.OrderWallets.Select(w => w.Wallet.BankAccountId).FirstOrDefault(),
                Price = x.OrderWallets.Select(w => w.Wallet.Price).FirstOrDefault(),
                HasRate = x.OrderRates.Any(),
                Isdelete = x.Isdelete,
                Orderstates = x.OrderStates.OrderBy(s => s.LogDate).Select(MapOrderState).ToList(),
                OrderDeliveries = x.OrderDeliveries.Select(d => MapDelivery(d, colorlist)).ToList(),
                // فقط یک سطح ChildOrders (دقیقاً مطابق رفتار نسخه‌ی قبلی) — برای نوه‌سفارش‌ها Include نداریم.
                ChildOrders = x.ChildOrders.Any() ? x.ChildOrders.Select(c => MapChildOrder(c, colorlist)).ToList() : null
            };
        }

        private OrderV2 MapChildOrder(Order c, List<ProductAttributeItemColor> colorlist)
        {
            return new OrderV2
            {
                Id = c.Id,
                BankOrderId = c.BankOrderId,
                CustomerOrderId = c.CustomerOrderId,
                InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(c.InsertDate),
                BankId = c.OrderWallets.Select(w => w.Wallet.BankAccountId).FirstOrDefault(),
                Price = c.OrderWallets.Select(w => w.Wallet.Price).FirstOrDefault(),
                HasRate = c.OrderRates.Any(),
                Isdelete = c.Isdelete,
                Orderstates = c.OrderStates.OrderBy(s => s.LogDate).Select(MapOrderState).ToList(),
                OrderDeliveries = c.OrderDeliveries.Select(d => MapDelivery(d, colorlist)).ToList()
            };
        }

        private OrderStateV2 MapOrderState(OrderState s)
        {
            return new OrderStateV2
            {
                Id = s.Id,
                state = s.state,
                LogDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(s.LogDate) + " - " + s.LogDate.ToString("HH:mm"),
                LogDateRaw = s.LogDate
            };
        }

        private OrderDeliveryV2 MapDelivery(OrderDelivery d, List<ProductAttributeItemColor> colorlist)
        {
            // اگه ادمین روشِ ارسال رو خودش انتخاب/عوض کرده باشه (AdminSelectedSendway)، همون نهایی و
            // معتبره؛ وگرنه همون روشی که مشتری موقعِ ثبتِ سفارش انتخاب کرده (ProductSendWay).
            string sendWayTitle = d.AdminSelectedSendway != null
                ? d.AdminSelectedSendway.Title
                : (d.ProductSendWay != null ? d.ProductSendWay.Title : null);

            return new OrderDeliveryV2
            {
                Id = d.Id,
                WalletAttributeWalles = d.WalletAttributeWallets.Select(w => new WalletAttributeWalletV2 { DataType = w.WalletAttribute.DataType }).ToList(),
                OrderRows = d.OrderRows.Select(r => MapOrderRow(r, colorlist)).ToList(),
                SendWayTitle = sendWayTitle,
                PredictDate = d.PredictDate.HasValue
                    ? CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsiWithoutYear(d.PredictDate.Value)
                    : null
            };
        }

        private OrderRowsV2 MapOrderRow(OrderRow r, List<ProductAttributeItemColor> colorlist)
        {
            return new OrderRowsV2
            {
                Id = r.Id,
                ProductId = r.ProductId,
                PageAddress = r.Product.PageAddress,
                Title = r.Product.Title,
                Quantity = r.Quantity,
                Disabled = r.Disabled,

                ProductAttributeSelectModelId = r.ProductPrice.ProductAttributeSelectModelId ?? null,
                ProductAttributeSelectModel = r.ProductPrice.ProductAttributeSelectModel != null ? r.ProductPrice.ProductAttributeSelectModel.Value : null,
                ProductAttributeSelectSizeId = r.ProductPrice.ProductAttributeSelectSizeId ?? null,
                ProductAttributeSelectSize = r.ProductPrice.ProductAttributeSelectSize != null ? r.ProductPrice.ProductAttributeSelectSize.Value : null,
                Unit = r.ProductPrice.ProductAttributeSelectSize != null ? r.ProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : null,
                ProductAttributeSelectColorId = r.ProductPrice.ProductAttributeSelectColorId ?? null,
                ProductAttributeSelectColor = r.ProductPrice.ProductAttributeSelectColor != null
                    ? colorlist.Where(a => a.Id == Convert.ToInt32(r.ProductPrice.ProductAttributeSelectColor.Value)).First().Color
                    : "",
                ImageFileName = GetOrderRowImageFileName(r)
            };
        }

        private string GetOrderRowImageFileName(OrderRow r)
        {
            if (r.ProductPrice.ProductImages.Any())
            {
                return r.ProductPrice.ProductImages.Any(i => i.IsMain)
                    ? r.ProductPrice.ProductImages.Where(i => i.IsMain).Select(i => i.Image.FileName).FirstOrDefault()
                    : r.ProductPrice.ProductImages.Select(i => i.Image.FileName).FirstOrDefault();
            }

            return r.Product.ProductPrices.Any(p => p.ProductImages.Any())
                ? r.Product.ProductPrices.Where(p => p.ProductImages.Any()).SelectMany(p => p.ProductImages).Select(i => i.Image.FileName).FirstOrDefault()
                : null;
        }
        #endregion

        public PagedList.IPagedList<OrderV2> GetAllOrderV2(string userid, string date, string keyword, int pageNumber, int pageSize)
        {
            var baseQuery = GetQueryList().AsNoTracking().Where(x => x.UserId == userid);
            baseQuery = ApplyOrderFilters(baseQuery, date, keyword);

            return PageAndHydrateOrders(baseQuery, pageNumber, pageSize);
        }

        public PagedList.IPagedList<OrderV2> GetCurrentOrderV2(string userid, string date, string keyword, int pageNumber, int pageSize)
        {
            var orderStates = from x in _orderStateService.GetQueryList().AsNoTracking()
                              where x.Order.UserId == userid
                              group x by x.OrderId into g
                              select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orderStates.Where(x => x.state > OrderStatus.تایید_پرداخت && x.state < OrderStatus.تحویل_داده_شده).Select(x => x.Key);

            var baseQuery = GetQueryList().AsNoTracking().Where(x => currentorder.Contains(x.Id) && x.OrderId == null);
            baseQuery = ApplyOrderFilters(baseQuery, date, keyword);

            return PageAndHydrateOrders(baseQuery, pageNumber, pageSize);
        }
        public PagedList.IPagedList<OrderV2> GetCancelOrdersV2(string userid, string date, string keyword, int pageNumber, int pageSize)
        {
            var orderStates = from x in _orderStateService.GetQueryList().AsNoTracking()
                              where x.Order.UserId == userid
                              group x by x.OrderId into g
                              select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orderStates.Where(x => x.state == OrderStatus.لغو_شده || x.state == OrderStatus.درخواست_لغو || x.state == OrderStatus.عدم_تایید_درخواست_لغو).Select(x => x.Key);

            var baseQuery = GetQueryList().AsNoTracking().Where(x => currentorder.Contains(x.Id) && x.OrderId == null);
            baseQuery = ApplyOrderFilters(baseQuery, date, keyword);

            return PageAndHydrateOrders(baseQuery, pageNumber, pageSize);
        }
        public PagedList.IPagedList<OrderV2> GetReturnedOrdersV2(string userid, string date, string keyword, int pageNumber, int pageSize)
        {
            var orderStates = from x in _orderStateService.GetQueryList().AsNoTracking()
                              where x.Order.IsActive && x.Order.OrderWallets.Any(s => s.Wallet.State == true) && x.Order.UserId == userid && !x.Order.OrderWallets.Any(s => s.Wallet.WalletAttributeWallets.Any(a => a.WalletAttribute.DataType == 23))
                              group x by x.OrderId into g
                              select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orderStates.Where(x => x.state == OrderStatus.مرجوعی || x.state == OrderStatus.عدم_تایید_درخواست_مرجوعی || x.state == OrderStatus.درخواست_مرجوعی || x.state == OrderStatus.جبران_مرجوعی).Select(x => x.Key);

            var baseQuery = GetQueryList().AsNoTracking().Where(x => currentorder.Contains(x.Id));
            baseQuery = ApplyOrderFilters(baseQuery, date, keyword);

            return PageAndHydrateOrders(baseQuery, pageNumber, pageSize);
        }

        // سه‌تب‌بندیِ جدیدِ صفحه‌ی «سفارشات من» (به‌جای ۴ تبِ قدیمی: همه/در حال انجام/مرجوعی/لغو شده).
        // مبنا همون الگوی MAX(state) ای هست که سه متدِ بالا هم ازش استفاده می‌کنن (آخرین وضعیتِ ثبت‌شده‌ی
        // هر سفارش)، پس هر سفارش دقیقاً توی یکی از این سه تب می‌افته:
        //   - جاری:        MAX(state) < تحویل‌داده‌شده
        //   - پرداخت‌نشده:  isdelete==true یا MAX(state) در {لغو‌شده, مرجوعی}
        //   - بسته‌شده:     باقیِ حالت‌ها (رسیده به تحویل‌داده‌شده یا بعدش، بدون لغو/مرجوعیِ قطعی) -
        //                    یعنی درخواست‌های لغو/مرجوعیِ رد/جبران‌شده هم اینجا می‌افتن، چون سفارش
        //                    نهایتاً باطل نشده.
        public PagedList.IPagedList<OrderV2> GetInProgressOrdersV2(string userid, string date, string keyword, int pageNumber, int pageSize, string sort = null, int? filterState = null)
        {
            var orderStates = from x in _orderStateService.GetQueryList().AsNoTracking()
                              where x.Order.UserId == userid
                              group x by x.OrderId into g
                              select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orderStates.Where(x => x.state < OrderStatus.تحویل_داده_شده).Select(x => x.Key);

            var baseQuery = GetQueryList().AsNoTracking().Where(x => currentorder.Contains(x.Id) && x.OrderId == null && !x.Isdelete);
            baseQuery = ApplyOrderFilters(baseQuery, date, keyword, filterState);

            return PageAndHydrateOrders(baseQuery, pageNumber, pageSize, sort);
        }

        public PagedList.IPagedList<OrderV2> GetClosedOrdersV2(string userid, string date, string keyword, int pageNumber, int pageSize, string sort = null, int? filterState = null)
        {
            var orderStates = from x in _orderStateService.GetQueryList().AsNoTracking()
                              where x.Order.UserId == userid
                              group x by x.OrderId into g
                              select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> currentorder = orderStates.Where(x => x.state >= OrderStatus.تحویل_داده_شده && x.state != OrderStatus.لغو_شده && x.state != OrderStatus.مرجوعی).Select(x => x.Key);

            var baseQuery = GetQueryList().AsNoTracking().Where(x => currentorder.Contains(x.Id) && x.OrderId == null && !x.Isdelete);
            baseQuery = ApplyOrderFilters(baseQuery, date, keyword, filterState);

            return PageAndHydrateOrders(baseQuery, pageNumber, pageSize, sort);
        }

        public PagedList.IPagedList<OrderV2> GetUnpaidOrdersV2(string userid, string date, string keyword, int pageNumber, int pageSize, string sort = null, int? filterState = null)
        {
            var orderStates = from x in _orderStateService.GetQueryList().AsNoTracking()
                              where x.Order.UserId == userid
                              group x by x.OrderId into g
                              select new { g.Key, state = g.Max(x => x.state) };
            IQueryable<Guid> cancelledOrReturned = orderStates.Where(x => x.state == OrderStatus.لغو_شده || x.state == OrderStatus.مرجوعی).Select(x => x.Key);

            var baseQuery = GetQueryList().AsNoTracking().Where(x => x.OrderId == null && (x.Isdelete || cancelledOrReturned.Contains(x.Id)));
            baseQuery = ApplyOrderFilters(baseQuery, date, keyword, filterState);

            return PageAndHydrateOrders(baseQuery, pageNumber, pageSize, sort);
        }
    }
}
