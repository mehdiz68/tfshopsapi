using CoreLib;
using CoreLib.Infrastructure;
using DataLayer;
using Domain;
using Domain.ViewModel;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Repository.Service
{
    public class ProductService : GenericRepository<Product>
    {
        private ProductCategoryService _productCategoryService;
        //private ProductPriceService _productPriceService;
        // private readonly StoreRowService _storeRowService;
        public ProductService(TfShopDbContext context, ProductCategoryService productCategoryService) : base(context)
        {
            _productCategoryService = productCategoryService;
            //_productPriceService = productPriceService;
        }

        public string packagename = "";
        public ProductPackageType ProductPackageType;
        public string offexpiredate = "";
        public int? offerId = null;
        public string offtitle = "";
        public long cprice = 0;
        public long coffvalue = 0;
        public long coffvaluefinal = 0;
        public double ctaxtvalue = 0;
        public bool chasoff = false;
        public bool ProductOfferType = false;
        public int offerQuantity = 0;
        public int offerMaxQuantity = 0;
        public short cofftype = 3;
        public string model, color,colortitle, size,garanty = "";

        public IEnumerable<ProductItemCompare> ProductItemListComparev1(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);
            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItemCompare()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress.ToLower(),
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                //  Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.FirstOrDefault().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                //avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                //countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                avgRankValue = x.RateAvg,
                countRankValue = x.CountAvg,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                PAttributeSelect = x.ProductAttributeSelects.Select(a => new PAttributeSelect {Id= a.ProductAttributeGroupSelect.AttributeId,ProductId= a.ProductId,DisplayOrder= a.DisplayOrder,Title= a.ProductAttributeGroupSelect.ProductAttribute.Title, Value =a.Value }),
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                TaxId = x.TaxId

            });
        }

        public IEnumerable<ProductItemCompare> ProductItemListCompare(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);
            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItemCompare()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress.ToLower(),
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                //  Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.FirstOrDefault().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                //avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                //countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                avgRankValue = x.RateAvg,
                countRankValue = x.CountAvg,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                ProductAttributeSelects = x.ProductAttributeSelects,
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                TaxId = x.TaxId

            });
        }
        public IEnumerable<ProductItem> ProductItemList(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0, string userid = null)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectModel").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductAttributeSelectSize").Include("ProductPrices.ProductAttributeSelectGaranty").Include("ProductPrices.ProductAttributeSelectweight").Include("ProductPrices.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);
            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItem()
            {
                Id = x.Id,
                ProductPriceId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id,
                Name = x.Name,
                model = x.ProductPrices.Any(a => a.IsDefault) ? GetModeColorSize(x.Name, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectModelId, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectModel, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectSizeId, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectSize, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectSizeId.HasValue ? x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectColorId, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectColor, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectGarantyId, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectGaranty, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectWeightId, x.ProductPrices.Where(a => a.IsDefault).First().ProductAttributeSelectweight) : "",
                size = size,
                color = color,
                LatinName = x.LatinName,
                Title = x.Title,
                ShortTitle = x.ProductPrices.Count > 1 ? x.Name + (x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectModelId.HasValue ? " مدل " + x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectModel.Value : "") : x.Title,
                PageAddress = x.PageAddress.ToLower(),
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                //  Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.FirstOrDefault().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                //avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                //countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                avgRankValue = x.RateAvg,
                countRankValue = x.CountAvg,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                //ProductCategories = _productCategoryService.GetCategoryDtosByQueryInput(x.ProductCategories.AsQueryable()),
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                prpriceCount = x.ProductPrices.Where(s => s.ProductAttributeSelectSizeId.HasValue || s.ProductAttributeSelectGarantyId.HasValue || s.ProductAttributeSelectWeightId.HasValue).Count(),
                UserFav = userid != null ? x.ProductFavorates.Any(s => s.UserId == userid) : false,
                Maxquantity= x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().MaxBasketCount


            });
        }

     
        public IEnumerable<ProductItemRanks> ProductItemListRanks(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Include("ProductRankSelects.ProductRankGroupSelect.ProductRank").Where(filter);
            q = q.Where(x => x.IsActive);
            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItemRanks()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress.ToLower(),
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                //  Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                //avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                //countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                avgRankValue = x.RateAvg,
                countRankValue = x.CountAvg,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                //ProductCategories = _productCategoryService.GetCategoryDtosByQueryInput(x.ProductCategories.AsQueryable()),
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                TaxId = x.TaxId,
                ProductRanks = x.ProductRankSelects.Select(s => s.ProductRankGroupSelect).Select(a => a.ProductRank),

            });
        }

        public IEnumerable<ProductItem> ProductItemListWithCategories(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("Brand").Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);

            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress.ToLower(),
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                //avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                //countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                avgRankValue = x.RateAvg,
                countRankValue = x.CountAvg,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                ProductCategories = x.ProductCategories,
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                TaxId = x.TaxId,
                prpriceCount = x.ProductPrices.Where(s => s.ProductAttributeSelectSizeId.HasValue || s.ProductAttributeSelectGarantyId.HasValue || s.ProductAttributeSelectWeightId.HasValue).Count()


            });
        }

        public TotobItem TorobList(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            TotobItem item = new TotobItem();
            try
            {

                var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices").Include("ProductPrices.ProductAttributeSelectGaranty").Include("ProductPrices.ProductState").Include("ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Where(filter);
                q = q.Where(x => x.IsActive);

                if (orderBy != null)
                    q = orderBy(q);

                if (takeRecorf > 0)
                    q = q.Skip(() => skipRecord).Take(takeRecorf);

                var ids = q.Select(x => x.Id).ToList();
                var result = q.Where(x => ids.Contains(x.Id)).ToList();

                item.count = Get(x => filter).Count();
                item.max_pages = Convert.ToInt32(Math.Ceiling((item.count * 1.0) / 100));
                item.products = result.Select(x => new TorobProduct()
                {
                    availability = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 ? "instock" : "" : "",
                    category_name = x.ProductCategories.Any() ? x.ProductCategories.FirstOrDefault().Name : "",
                    old_price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                    current_price = x.ProductPrices.Any(a => a.IsDefault) ? cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.Any() ? x.ProductCategories.First().Id : 0, x.BrandId, cprice) : 0,
                    guarantee = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGarantyId != null ? GetGarantyItem(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGaranty.Value) : null : "",
                    image_link = x.ProductPrices.Any() ? "https://www.tfshops.com/Content/UploadFiles/" + GetMainImageFileName(x.Id) : "",
                    page_url = "https://www.tfshops.com/tfp/" + x.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(x.PageAddress.ToLower()),
                    page_unique = x.Id,
                    short_desc = !String.IsNullOrEmpty(x.Descr) ? Regex.Replace(x.Descr, "<.*?>", String.Empty).Replace("\"", "") : "",
                    subtitle = !String.IsNullOrEmpty(x.LatinName) ? Regex.Replace(x.LatinName, "<.*?>", String.Empty).Replace("\"", "") : "",
                    title = !String.IsNullOrEmpty(x.Title) ? Regex.Replace(x.Title, "<.*?>", String.Empty).Replace("\"", "") : "",
                    spec = x.ProductAttributeSelects.Where(s => s.ProductAttributeGroupSelect.AttributeId > 7 && s.ProductAttributeGroupSelect.ProductAttribute.PriceEffect == false).Select(s => new { Title = (s.ProductAttributeGroupSelect.ProductAttribute.Title == "" ? s.ProductAttributeGroupSelect.ProductAttribute.Name : s.ProductAttributeGroupSelect.ProductAttribute.Title), Value = s.Value }).GroupBy(s => s.Title).ToDictionary(t => t.Key, t => string.Join(", ", t.Where(a => a.Title == t.Key).Select(a => Regex.Replace(a.Value, "<.*?>", String.Empty).Replace("\"", ""))))

                }).ToList();
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                var pp = new List<TorobProduct>(); pp.Add(new TorobProduct() { title = ex.Message });
                return new TotobItem() { count = 0, max_pages = 0, products = pp };
            }

            return item;
        }
        public TotobItem TorobListPagging(Expression<Func<Domain.Product, bool>> filter, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy, int skipRecord, int takeRecorf)
        {
            TotobItem item = new TotobItem();
            try
            {

                var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices").Include("ProductPrices.ProductAttributeSelectGaranty").Include("ProductPrices.ProductState").Include("ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Where(filter);
                q = q.Where(x => x.IsActive);
                q = orderBy(q);
                q = q.Skip(() => skipRecord).Take(takeRecorf);
                var ids = q.Select(x => x.Id).ToList();
                var result = q.Where(x => ids.Contains(x.Id)).ToList();

                item.count = Get(x => filter).Count();
                item.max_pages = Convert.ToInt32(Math.Ceiling((item.count * 1.0) / 100));
                item.products = result.Select(x => new TorobProduct()
                {
                    availability = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 ? "instock" : "" : "",
                    category_name = x.ProductCategories.Any() ? x.ProductCategories.FirstOrDefault().Name : "",
                    old_price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                    current_price = x.ProductPrices.Any(a => a.IsDefault) ? cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.Any() ? x.ProductCategories.First().Id : 0, x.BrandId, cprice) : 0,
                    guarantee = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGarantyId != null ? GetGarantyItem(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGaranty.Value) : null : "",
                    image_link = x.ProductPrices.Any() ? "https://www.tfshops.com/Content/UploadFiles/" + GetMainImageFileName(x.Id) : "",
                    page_url = "https://www.tfshops.com/tfp/" + x.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(x.PageAddress.ToLower()),
                    page_unique = x.Id,
                    short_desc = !String.IsNullOrEmpty(x.Descr) ? Regex.Replace(x.Descr, "<.*?>", String.Empty).Replace("\"", "") : "",
                    subtitle = !String.IsNullOrEmpty(x.LatinName) ? Regex.Replace(x.LatinName, "<.*?>", String.Empty).Replace("\"", "") : "",
                    title = !String.IsNullOrEmpty(x.Title) ? Regex.Replace(x.Title, "<.*?>", String.Empty).Replace("\"", "") : "",
                    spec = x.ProductAttributeSelects.Where(s => s.ProductAttributeGroupSelect.AttributeId > 7 && s.ProductAttributeGroupSelect.ProductAttribute.PriceEffect == false).Select(s => new { Title = (s.ProductAttributeGroupSelect.ProductAttribute.Title == "" ? s.ProductAttributeGroupSelect.ProductAttribute.Name : s.ProductAttributeGroupSelect.ProductAttribute.Title), Value = s.Value }).GroupBy(s => s.Title).ToDictionary(t => t.Key, t => string.Join(", ", t.Where(a => a.Title == t.Key).Select(a => Regex.Replace(a.Value, "<.*?>", String.Empty).Replace("\"", ""))))

                }).ToList();
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                var pp = new List<TorobProduct>(); pp.Add(new TorobProduct() { title = ex.Message });
                return new TotobItem() { count = 0, max_pages = 0, products = pp };
            }

            return item;
        }
        public ZarehbinItem ZarebinList(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            ZarehbinItem item = new ZarehbinItem();
            try
            {

                var q = GetQueryList().AsNoTracking().Include("ProductImages.Image").Include("ProductCategories").Include("ProductPrices").Include("ProductPrices.ProductAttributeSelectGaranty").Include("ProductPrices.ProductState").Include("ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Where(filter);
                q = q.Where(x => x.IsActive);

                if (orderBy != null)
                    q = orderBy(q);

                if (takeRecorf > 0)
                    q = q.Skip(() => skipRecord).Take(takeRecorf);

                var ids = q.Select(x => x.Id).ToList();
                var result = q.Where(x => ids.Contains(x.Id)).ToList();

                item.count = Get(x => filter).Count();
                item.total_pages_count = Convert.ToInt32(Math.Ceiling((item.count * 1.0) / 100));
                item.products = result.Select(x => new ZarehbinProduct()
                {
                    availability = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 ? "instock" : "" : "",
                    categories = x.ProductCategories.Any() ? new List<string>() { x.ProductCategories.FirstOrDefault().Name } : null,
                    old_price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                    current_price = x.ProductPrices.Any(a => a.IsDefault) ? cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.Any() ? x.ProductCategories.First().Id : 0, x.BrandId, cprice) : 0,
                    guarantee = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGarantyId != null ? GetGarantyItem(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGaranty.Value) : null : "",
                    image_link = x.ProductPrices.Any() ? "https://www.tfshops.com/Content/UploadFiles/" + GetMainImageFileName(x.Id) : "",
                    image_links = x.ProductImages.Select(a => "https://www.tfshops.com/Content/UploadFiles/" + a.Image.FileName).ToList(),
                    page_url = "https://www.tfshops.com/tfp/" + x.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(x.PageAddress.ToLower()),
                    id = x.Id,
                    short_desc = !String.IsNullOrEmpty(x.Descr) ? Regex.Replace(x.Descr, "<.*?>", String.Empty).Replace("\"", "") : "",
                    subtitle = !String.IsNullOrEmpty(x.LatinName) ? Regex.Replace(x.LatinName, "<.*?>", String.Empty).Replace("\"", "") : "",
                    title = !String.IsNullOrEmpty(x.Title) ? Regex.Replace(x.Title, "<.*?>", String.Empty).Replace("\"", "") : "",
                    spec = x.ProductAttributeSelects.Where(s => s.ProductAttributeGroupSelect.AttributeId > 7 && s.ProductAttributeGroupSelect.ProductAttribute.PriceEffect == false).Select(s => new { Title = (s.ProductAttributeGroupSelect.ProductAttribute.Title == "" ? s.ProductAttributeGroupSelect.ProductAttribute.Name : s.ProductAttributeGroupSelect.ProductAttribute.Title), Value = s.Value }).GroupBy(s => s.Title).ToDictionary(t => t.Key, t => string.Join(", ", t.Where(a => a.Title == t.Key).Select(a => Regex.Replace(a.Value, "<.*?>", String.Empty).Replace("\"", "")))),
                    registry = ""

                }).ToList();
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                var pp = new List<ZarehbinProduct>(); pp.Add(new ZarehbinProduct() { title = ex.Message });
                return new ZarehbinItem() { count = 0, total_pages_count = 0, products = pp };
            }

            return item;
        }
        public ZarehbinItem ZarebinListPagging(Expression<Func<Domain.Product, bool>> filter, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy, int skipRecord, int takeRecorf)
        {
            ZarehbinItem item = new ZarehbinItem();
            try
            {

                var q = GetQueryList().AsNoTracking().Include("ProductImages.Image").Include("ProductCategories").Include("ProductPrices").Include("ProductPrices.ProductAttributeSelectGaranty").Include("ProductPrices.ProductState").Include("ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Where(filter);
                q = q.Where(x => x.IsActive);
                q = orderBy(q);
                q = q.Skip(() => skipRecord).Take(takeRecorf);
                var ids = q.Select(x => x.Id).ToList();
                var result = q.Where(x => ids.Contains(x.Id)).ToList();

                item.count = Get(x => filter).Count();
                item.total_pages_count = Convert.ToInt32(Math.Ceiling((item.count * 1.0) / 100));
                item.products = result.Select(x => new ZarehbinProduct()
                {
                    availability = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 ? "instock" : "" : "",
                    categories = x.ProductCategories.Any() ? new List<string>() { x.ProductCategories.FirstOrDefault().Name } : null,
                    old_price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                    current_price = x.ProductPrices.Any(a => a.IsDefault) ? cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.Any() ? x.ProductCategories.First().Id : 0, x.BrandId, cprice) : 0,
                    guarantee = x.ProductPrices.Any(a => a.IsDefault) ? x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGarantyId != null ? GetGarantyItem(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductAttributeSelectGaranty.Value) : null : "",
                    image_link = x.ProductPrices.Any() ? "https://www.tfshops.com/Content/UploadFiles/" + GetMainImageFileName(x.Id) : "",
                    image_links = x.ProductImages.Select(a => "https://www.tfshops.com/Content/UploadFiles/" + a.Image.FileName).ToList(),
                    page_url = "https://www.tfshops.com/tfp/" + x.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(x.PageAddress.ToLower()),
                    id = x.Id,
                    short_desc = !String.IsNullOrEmpty(x.Descr) ? Regex.Replace(x.Descr, "<.*?>", String.Empty).Replace("\"", "") : "",
                    subtitle = !String.IsNullOrEmpty(x.LatinName) ? Regex.Replace(x.LatinName, "<.*?>", String.Empty).Replace("\"", "") : "",
                    title = !String.IsNullOrEmpty(x.Title) ? Regex.Replace(x.Title, "<.*?>", String.Empty).Replace("\"", "") : "",
                    spec = x.ProductAttributeSelects.Where(s => s.ProductAttributeGroupSelect.AttributeId > 7 && s.ProductAttributeGroupSelect.ProductAttribute.PriceEffect == false).Select(s => new { Title = (s.ProductAttributeGroupSelect.ProductAttribute.Title == "" ? s.ProductAttributeGroupSelect.ProductAttribute.Name : s.ProductAttributeGroupSelect.ProductAttribute.Title), Value = s.Value }).GroupBy(s => s.Title).ToDictionary(t => t.Key, t => string.Join(", ", t.Where(a => a.Title == t.Key).Select(a => Regex.Replace(a.Value, "<.*?>", String.Empty).Replace("\"", "")))),
                    registry = ""

                }).ToList();
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                var pp = new List<ZarehbinProduct>(); pp.Add(new ZarehbinProduct() { title = ex.Message });
                return new ZarehbinItem() { count = 0, total_pages_count = 0, products = pp };
            }

            return item;
        }

        public YektanetProduct YektanetProductDetail(int id)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("Brand").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(x => x.Id == id);
            var result = q.ToList();


            return result.Select(x => new YektanetProduct()
            {
                sku = "C-" + x.Id,
                image = "https://www.tfshops.com/Content/UploadFiles/" + GetMainImageFileName(x.Id),
                title = x.Title,
                isAvailable = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2,
                category = new List<string>() { x.ProductCategories.FirstOrDefault().Name },
                price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                discount = coffvalue,
                currency = "IRT",
                brand = x.Brand.Name,
                averageVote = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count() : 0,
                totalVotes = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                expiration = 0

            }).SingleOrDefault();


        }

        public List<YektanetBuyProduct> YektanetBuyProduct(IEnumerable<OrderRow> OrderRows)
        {


            return OrderRows.Select(x => new YektanetBuyProduct()
            {
                sku = "C-" + x.ProductId,
                quantity = x.Quantity,
                price = x.Price * x.Quantity,
                currency = "IRT"

            }).ToList();


        }

        public IEnumerable<ProductItem> ProductItemList2(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {

            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);
            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress.ToLower(),
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                // //  Brand = x.Brand,
                MainImageFileName = GetMainImageFileName(x.Id),
                // //OtherImageFileName = GetOtherImageFileName(x.Id),
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                ////Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                //avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                // countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                // Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                // //ProductCategories = _productCategoryService.GetCategoryDtosByQueryInput(x.ProductCategories.AsQueryable()),
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,
                TaxId = x.TaxId,
                prpriceCount = x.ProductPrices.Where(s => s.ProductAttributeSelectSizeId.HasValue || s.ProductAttributeSelectGarantyId.HasValue || s.ProductAttributeSelectWeightId.HasValue).Count()


            });
        }


        public IEnumerable<ProductItem> ProductItemListPrice(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {

            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices").Include("ProductPrices.ProductState").Where(filter);
            q = q.Where(x => x.IsActive);

            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();



            return q.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.First().Id, x.BrandId, cprice),
                prpriceCount = x.ProductPrices.Where(s => s.ProductAttributeSelectSizeId.HasValue || s.ProductAttributeSelectGarantyId.HasValue || s.ProductAttributeSelectWeightId.HasValue).Count()


            });
        }

        public ProductItem ProductItem(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);

            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Name,
                LatinName = x.LatinName,
                Title = x.Title,
                PageAddress = x.PageAddress.ToLower(),
                Descr = x.Descr,
                Code = x.Code,
                ProductStateId = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId.Value,
                ProductStateTitle = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductState.Title,
                ProductIcon = x.ProductIcon,
                //    Brand = x.Brand,
                Price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId),
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Id, x.ProductCategories.FirstOrDefault().Id, x.BrandId, cprice),
                offValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId < 3 ? chasoff : false,
                //avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                //countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                avgRankValue = x.RateAvg,
                countRankValue = x.CountAvg,
                Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                //ProductCategories = _productCategoryService.GetCategoryDtosByQueryInput(x.ProductCategories.AsQueryable()),
                Favorates = x.ProductFavorates.Count,
                LetmeKnows = x.ProductLetmeknows.Count,


            }).SingleOrDefault();
        }
        public ProductItemvm ProductItemv2(Expression<Func<Domain.Product, bool>> filter = null, Func<IQueryable<Domain.Product>, IOrderedQueryable<Domain.Product>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = GetQueryList().AsNoTracking().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectColor").Include("ProductPrices.ProductState").Include("ProductIcon").Include("ProductRankSelects").Include("ProductFavorates").Include("ProductLetmeknows").Include("ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);

            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItemvm()
            {
                Id = x.Id,
                Name = x.Name,
                Title = x.Title,
                MainImageFileName = GetMainImageFileName(x.Id)               

            }).SingleOrDefault();
        }

        public ProductItem ProductPriceItem(Expression<Func<Domain.ProductPrice, bool>> filter = null, Func<IQueryable<Domain.ProductPrice>, IOrderedQueryable<Domain.ProductPrice>> orderBy = null, int skipRecord = 0, int takeRecorf = 0)
        {
            var q = context.ProductPrices.AsQueryable().AsNoTracking().Include("Product").Include("Product.ProductCategories").Include("ProductAttributeSelectColor").Include("ProductState").Include("Product.ProductIcon").Include("Product.ProductRankSelects").Include("Product.ProductFavorates").Include("Product.ProductLetmeknows").Include("Product.ProductRankSelects.ProductRankSelectValues").Where(filter);
            q = q.Where(x => x.IsActive);

            if (orderBy != null)
                q = orderBy(q);

            if (takeRecorf > 0)
                q = q.Skip(() => skipRecord).Take(takeRecorf);
            var result = q.ToList();

            return result.Select(x => new ProductItem()
            {
                Id = x.Id,
                Name = x.Product.Name,
                LatinName = x.Product.LatinName,
                Title = x.Product.Title,
                PageAddress = x.Product.PageAddress.ToLower(),
                Descr = x.Product.Descr,
                Code = x.code,
                ProductStateId = x.ProductStateId.Value,
                ProductStateTitle = x.ProductState.Title,
                ProductIcon = x.Product.ProductIcon,
                //    Brand = x.Brand,
                Price = GetPrice(x.Id, x.Price, x.Product.TaxId),
                MainImageFileName = GetMainImageFileName(x.Id),
                //OtherImageFileName = GetOtherImageFileName(x.Id),
                //Price = x.ProductPrices.Any(a => a.IsDefault) ? GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) : 0,
                finalPrice = cprice - GetOff(x.Id, x.Product.ProductCategories.FirstOrDefault().Id, x.Product.BrandId, cprice),
                offValue = x.ProductStateId < 3 ? coffvalue : 0,
                offFinalValue = x.ProductStateId < 3 ? coffvaluefinal : 0,
                tax = ctaxtvalue,
                offtype = cofftype,
                hasoff = x.ProductStateId < 3 ? chasoff : false,
                //avgRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count(),
                //countRankValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 0,
                avgRankValue = x.Product.RateAvg,
                countRankValue = x.Product.CountAvg,
                //Colors = x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Any() ? GetColor(x.ProductPrices.Where(s => s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                //ProductCategories = _productCategoryService.GetCategoryDtosByQueryInput(x.ProductCategories.AsQueryable()),
                Favorates = x.Product.ProductFavorates.Count,
                LetmeKnows = x.Product.ProductLetmeknows.Count,


            }).SingleOrDefault();
        }



        public List<string> GetOtherImageFileName(int id)
        {
            var sqlQuery = @"                SELECT  attachments.FileName FROM ProductPrices AS pp
                       LEFT OUTER   JOIN ProductImages ON pp.ProductId = ProductImages.ProductId
                        LEFT OUTER JOIN attachments ON ProductImages.AttachementId = attachments.Id

                        WHERE 
                         pp.ProductId = 15 AND pp.IsDefault =1 AND  ProductImages.IsImage = 1 AND ProductImages.IsMain = 0
                        ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc";

            var queryResult = context.Database.SqlQuery<string>(sqlQuery).AsQueryable();
            return queryResult.ToList();
        }
        /// <summary>
        /// کوئری برای ئیدا کردن اولین تصویر یک کالا
        /// </summary>
        /// <param name="productPrices"></param>
        /// <returns></returns>
        public string GetMainImageFileName(int pId)
        {

            var sqlQuery = @" 
                SELECT top(1) FIRST_VALUE( attachments.FileName) OVER ( ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc)  FROM ProductPrices AS pp
                       LEFT OUTER   JOIN ProductImages ON pp.Id = ProductImages.ProductPriceId
                        LEFT OUTER JOIN attachments ON ProductImages.AttachementId = attachments.Id

                        WHERE 
                         pp.ProductId =  " + pId + @" AND  ProductImages.IsImage = 1
                        ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc";
            var queryResult = context.Database.SqlQuery<string>(sqlQuery).AsQueryable();
            return queryResult.FirstOrDefault();

        }
        public string GetMainProductPriceImageFileName(int pId, int productid)
        {

            var sqlQuery = @" 
                SELECT top(1) FIRST_VALUE( attachments.FileName) OVER ( ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc)  FROM ProductPrices AS pp
                       LEFT OUTER   JOIN ProductImages ON pp.Id = ProductImages.ProductPriceId
                        LEFT OUTER JOIN attachments ON ProductImages.AttachementId = attachments.Id

                        WHERE 
                         pp.Id =  " + pId + @" AND  ProductImages.IsImage = 1
                        ORDER BY pp.IsDefault desc , ProductImages.IsMain  desc";
            var queryResult = context.Database.SqlQuery<string>(sqlQuery).AsQueryable();
            if (queryResult.FirstOrDefault() != null)
                return queryResult.FirstOrDefault();
            else
                return GetMainImageFileName(productid);

        }

        public Domain.ProductImage GetMainImage(IEnumerable<Domain.ProductPrice> productPrices)
        {
            if (productPrices.Any(x => x.ProductImages.Any()))
            {
                if (productPrices.Any(x => x.IsDefault))
                {
                    if (productPrices.Where(x => x.IsDefault).First().ProductImages.Any())
                    {
                        if (productPrices.Where(x => x.IsDefault).First().ProductImages.Any(x => x.IsMain)) // تنوع پیش فرض ، عکس اصلی
                        {
                            return productPrices.Where(x => x.IsDefault).First().ProductImages.Where(x => x.IsMain).First();
                        }
                        else // تنوع پیش فرض ، اولین عکس
                        {
                            return productPrices.Where(x => x.IsDefault).First().ProductImages.First();
                        }
                    }
                    else // اولین تنوع، اولین عکس
                    {

                        return productPrices.SelectMany(x => x.ProductImages).First();
                    }
                }
                else // اولین تنوع، اولین عکس
                {
                    return productPrices.SelectMany(x => x.ProductImages).First();
                }
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<string> GetColor(IEnumerable<string> ColorIds)
        {
            if (ColorIds != null)
            {
                return context.ProductAttributeItemColors.Where(x => ColorIds.Contains(x.Id.ToString())).Select(x => x.Value);
            }
            else
                return null;
        }
        public string GetColor(int ColorId)
        {

            return context.ProductAttributeItemColors.Where(x => x.Id == ColorId).Select(x => x.Value).FirstOrDefault();

        }
        public ProductAttributeItemColor GetColorItem(int ColorId)
        {

            return context.ProductAttributeItemColors.Where(x => x.Id == ColorId).FirstOrDefault();
        }
        public IEnumerable<ProductAttributeItemColor> GetColorItems(IEnumerable<string> ColorIds)
        {
            if (ColorIds != null)
            {
                return context.ProductAttributeItemColors.Where(x => ColorIds.Contains(x.Id.ToString()));
            }
            else
                return null;
        }
        public string GetGaranty(int garantyId)
        {

            return context.ProductAttributeItems.Where(x => x.Id == garantyId).Select(x => x.Value).FirstOrDefault();

        }
        private IEnumerable<string> GetGaranty(IEnumerable<string> GarantyIds)
        {
            if (GarantyIds != null)
            {
                return context.ProductAttributeItems.Where(x => GarantyIds.Contains(x.Id.ToString())).Select(x => x.Value);
            }
            else
                return null;
        }
        public ProductAttributeItem GetGarantyItem(int GarantyId)
        {

            return context.ProductAttributeItems.Where(x => x.Id == GarantyId).FirstOrDefault();

        }
        public IEnumerable<ProductAttributeItem> GetGarantyItems(IEnumerable<string> GarantyIds)
        {
            if (GarantyIds != null)
            {
                return context.ProductAttributeItems.Where(x => GarantyIds.Contains(x.Id.ToString()));
            }
            else
                return null;
        }
        public string GetGarantyItem(string garantyId)
        {
            if (garantyId != null)
            {
                return context.ProductAttributeItems.Where(x => x.Id.ToString() == garantyId).First().Value;
            }
            else
                return null;
        }
        public long GetPrice(int ProductId, long price, int? taxid)
        {

            //if (!taxid.HasValue)
            //{
            //    ctaxtvalue = 0;
            //    cprice = price;
            //    cprice = Convert.ToInt64(Math.Ceiling(cprice * 0.001) * 1000);
            //    return cprice;
            //}
            //else
            //{
            //    ctaxtvalue = ((context.Taxes.Find(taxid.Value).TaxPercent * 0.01) * price);
            //    double prc = ctaxtvalue + price;
            //    cprice = Convert.ToInt64(Math.Ceiling(prc * 0.001) * 1000);
            //    return cprice;

            //}
            ctaxtvalue = 0;
            cprice = price;
            cprice = Convert.ToInt64(Math.Ceiling(cprice * 0.001) * 1000);
            return cprice;

        }

        public GetLongRange GetPriceRaneg(int catIds)
        {
            var prices = SqlQueryLong("exec GetPriceProductCategory @catid", new SqlParameter("@catid", catIds)).ToList();
            //var sss = context.ProductPrices.Where(c => c.IsDefault && c.IsActive && c.Product.IsActive && c.Product.state==4 && c.Product.LanguageId==1 && c.Product.ProductCategories.Any(s => catIds.Contains(s.Id))).Select(s => s.Price);
            if (prices.Any())
            {
                return new GetLongRange
                {
                    R1 = prices.Min(),
                    R2 = prices.Max()
                };
            }
            else
            {
                return new GetLongRange
                {
                    R1 = 0,
                    R2 = 0
                };
            }
        }
        public GetLongRange GetPriceRanegGetPriceBrand(int id)
        {
            var prices = SqlQueryLong("exec GetPriceBrand @id", new SqlParameter("@id", id)).ToList();
            if (prices.Any())
            {
                return new GetLongRange
                {
                    R1 = prices.Min(),
                    R2 = prices.Max()
                };
            }
            else
                return new GetLongRange
                {
                    R1 = 0,
                    R2 = 0
                };

        }
        public GetLongRange GetPriceRanegGetPriceTag(int tagId)
        {
            var prices = SqlQueryLong("exec GetPriceTag @TagId", new SqlParameter("@TagId", tagId)).ToList();
            if (prices.Any())
            {
                return new GetLongRange
                {
                    R1 = prices.Min(),
                    R2 = prices.Max()
                };
            }
            else
                return new GetLongRange
                {
                    R1 = 0,
                    R2 = 0
                };

        }

        public long GetOff(int ProductPriceId, int CatId, int? BrandId, long price)
        {
            offerId = null;
            chasoff = false;
            var productoffer = context.ProductOffers.Include("Offer").Include("ProductPrice").Where(s => s.Quantity > 0 && s.Value > 0 && s.ProductPrice.ProductStateId < 3 && s.ProductPriceId == ProductPriceId && s.Offer.IsActive && s.Offer.state == true && ((s.Offer.ExpireDate != null && s.Offer.ExpireDate >= DateTime.Now) || s.Offer.ExpireDate == null) && ((s.Offer.StartDate != null && s.Offer.StartDate <= DateTime.Now) || s.Offer.StartDate == null)).FirstOrDefault();
            if (productoffer != null)
            {
                offexpiredate = productoffer.Offer.ExpireDate.HasValue ? productoffer.Offer.ExpireDate.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture) + " " + productoffer.Offer.ExpireDate.Value.ToString("HH:mm:ss", CultureInfo.InvariantCulture) : "";
                ProductOfferType = productoffer.Offer.CodeTypeValueCode == 2 ? false : true;
                offtitle = productoffer.Offer.Title;
                cofftype = productoffer.CodeType;
                offerQuantity = productoffer.Quantity;
                offerMaxQuantity = productoffer.MaxBasketCount;
                chasoff = true;
                offerId = productoffer.Id;
                return ComputeOffVaule(productoffer.CodeType, productoffer.Value, price);

            }
            else
            {
                chasoff = false;
                cofftype = 3;
                coffvalue = 0;
                coffvaluefinal = 0;
                return 0;
            }
        }
        public long ComputeOffVaule(short codeType, int value, long price)
        {
            if (codeType == 2)
            {
                coffvaluefinal = Convert.ToInt64(price * (value * 0.01));
                coffvaluefinal = Convert.ToInt64(Math.Ceiling(coffvaluefinal * 0.001) * 1000);
                coffvalue = value;
                return coffvaluefinal;
            }
            else if (codeType == 1)
            {
                coffvaluefinal = value;
                coffvaluefinal = Convert.ToInt64(Math.Ceiling(coffvaluefinal * 0.001) * 1000);
                coffvalue = value;
                return coffvaluefinal;
            }
            else
            {
                coffvalue = 0;
                coffvaluefinal = 0;
                return 0;
            }
        }

        public string GerProductBreadcrumb(int productId)
        {
            string prefix = "TFS";
            string breadcrumb = "";
            List<ProductCategory> breadcrumbList = new List<ProductCategory>();
            var prcats = GetQueryList().Include("ProductCategories").Include("ProductPrices.ProductAttributeSelectModel").Include("ProductPrices.ProductAttributeSelectSize").Include("ProductPrices.ProductAttributeSelectColor").AsNoTracking().Where(x => x.Id == productId).Select(x => new { ProductCategory = x.ProductCategories.FirstOrDefault(), Id = x.Id, PrPageAddress = x.PageAddress.ToLower(), PrTitle = x.Title, PrName = x.Name, PrModel = x.ProductPrices.Where(s => s.IsDefault).FirstOrDefault().ProductAttributeSelectModelId.HasValue ? x.ProductPrices.Where(s => s.IsDefault).FirstOrDefault().ProductAttributeSelectModel.Value : "", PrSize = x.ProductPrices.Where(s => s.IsDefault).FirstOrDefault().ProductAttributeSelectSizeId.HasValue ? x.ProductPrices.Where(s => s.IsDefault).FirstOrDefault().ProductAttributeSelectSize.Value : "", PrColor = x.ProductPrices.Where(s => s.IsDefault).FirstOrDefault().ProductAttributeSelectColorId.HasValue ? x.ProductPrices.Where(s => s.IsDefault).FirstOrDefault().ProductAttributeSelectColor.Value : "" }).ToList();
            if (!prcats.First().ProductCategory.ParrentId.HasValue)
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFC/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='1' /></li>", prcats.First().ProductCategory.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.First().ProductCategory.PageAddress.ToLower()), prcats.First().ProductCategory.Name);
            else
            {
                string colorrr = prcats.First().PrColor;
                if (!String.IsNullOrEmpty(prcats.First().PrColor))
                {
                    int colorr = int.Parse(prcats.First().PrColor);
                    colorrr = context.ProductAttributeItemColors.Where(x => x.Id == colorr).First().Color;
                }
                var parent = prcats.First().ProductCategory.ParentCat;
                while (parent != null)
                {
                    breadcrumbList.Add(new ProductCategory() { Id = parent.Id, Name = parent.Name, PageAddress = parent.PageAddress.ToLower(), ParrentId = parent.ParrentId });
                    parent = parent.ParentCat;
                }
                int i = 1;
                foreach (var item in breadcrumbList.OrderBy(x => x.Id))
                {

                    if (!item.ParrentId.HasValue)
                        prefix = "TFC";
                    else
                        prefix = "TFS";

                    breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/" + prefix + "/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", item.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(item.PageAddress.ToLower()), item.Name, i);
                    i++;
                }
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFS/{0}/{1}'><span itemprop='name'>{2}</span></a><meta itemprop='position' content='{3}' /></li>", prcats.First().ProductCategory.Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.First().ProductCategory.PageAddress.ToLower()), prcats.First().ProductCategory.Name, i);
                breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFP/{0}/{1}'><span itemprop='name'><span>{2}</span><span>{3}</span><span>{4}</span><span>{5}</span><span>{6}</span></span></a><meta itemprop='position' content='{7}' /></li>", prcats.First().Id, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(prcats.First().PrPageAddress.ToLower()), prcats.First().PrName, " مدل ", prcats.First().PrModel, (!String.IsNullOrEmpty(prcats.First().PrSize) ? " سایز " + prcats.First().PrSize : ""), (colorrr), i + 1);
            }
            return breadcrumb;
        }


        public string GerProductBreadcrumbBrand(int BrandId, string Title, string Name, int? CatId)
        {
            string breadcrumb = "";
            //if (CatId.HasValue)
            //    breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><a itemprop='item' href='/TFB/{0}/{1}/{2}'><span itemprop='name'>{3}</span></a><meta itemprop='position' content='{4}' /></li>", BrandId, CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(Name), CatId.Value, Name, 1);
            //else
            breadcrumb += string.Format("<li itemprop='itemListElement' itemscope='' itemtype='http://schema.org/ListItem' class='breadcrumb-item'><span itemprop='item'><span itemprop='name'>{0}</span><meta itemprop='position' content='{1}' /></span></li>", Name, 1);
            return breadcrumb;
        }


        public async Task addProductFavorate(int productid)
        {
            var pr = GetByID(productid);
            pr.FavCount++;
            Update(pr);
            await context.SaveChangesAsync();
        }
        public async Task removeProductFavorate(int productid)
        {
            var pr = GetByID(productid);
            pr.FavCount--;
            Update(pr);
            await context.SaveChangesAsync();
        }
        public ProductReviewGoogleList GetProductReviewGoogleList(int productid)
        {
            return GetByReturnQueryable(x => new ProductReviewGoogleList
            {
                @context = "https://schema.org/",
                @type = "Product",
                name = x.Title,
                image = x.ProductImages.Where(sx => sx.IsImage).OrderBy(s => s.IsMain).Select(s => "https://www.tfshops.com/Content/UploadFiles/" + s.Image.FileName),
                //image = x.ProductImages.Where(sx => sx.IsImage).OrderBy(s => s.IsMain).Select(s => "https://www.tfshops.com/tf-Products/" + s.Image.FileName.GetFileName() + "/" + s.Image.FileName.GetFolderName() + "/1280/1280/LG"),
                description = x.Abstract,
                sku = x.Id,
                brand = new brand() { @type = "Brand", name = x.Brand.Name },
                review = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.User).Select(s => new reviewitem() { @type = "Review", author = new author { type = "Person", name = s.Key != null ? !String.IsNullOrEmpty(s.Key.FirstName) ? (s.Key.FirstName + " " + s.Key.LastName) : "مشتری" : "کارشناس" }, reviewBody = x.ProductComments.Where(a => a.UserId == s.Key.Id).Any() ? x.ProductComments.Where(a => a.UserId == s.Key.Id).FirstOrDefault().Title : "", reviewRating = new reviewRating() { @type = "Rating", ratingValue = s.Average(a => a.Value), bestRating = 5, worstRating = 1 } }),
                aggregateRating = new aggregateRating() { @type = "AggregateRating", ratingValue = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? Math.Round((x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count()), 2) : 3, reviewCount = x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 1 },
                offers = new offers()
                {
                    @type = "Offer",
                    url = "https://www.tfshops.com/tfp/" + x.Id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(x.PageAddress.ToLower()),
                    priceCurrency = "IRR",
                    price = GetPrice(x.Id, x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().Price, x.TaxId) * 10,
                    availability = x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 1 || x.ProductPrices.Where(a => a.IsDefault).FirstOrDefault().ProductStateId == 2 ? "https://schema.org/InStock" : "https://schema.org/OutOfStock",
                    priceValidUntil = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm")
                }

            }, x => x.Id == productid && x.IsActive && x.ProductRankSelects.Any(), null, "ProductPrices,ProductIcon,ProductImages.Image,Brand,ProductRankSelects.ProductRankSelectValues.User,ProductComments").SingleOrDefault();

        }

        public List<ProductVideoGoogleList> GetProductVideoGoogleList(int productid)
        {
            List<ProductVideoGoogleList> ProductVideoGoogleLists = new List<ProductVideoGoogleList>();
            if (Any(x => x.Id, x => x.Video.HasValue && x.VideoCover.HasValue && x.Id == productid))
            {
                var productVideo = GetQueryList().AsNoTracking().Include(x => x.VideoAttachment).Include(x => x.VideoAttachmentCover).Where(x => x.Video.HasValue && x.VideoCover.HasValue && x.Id == productid).SingleOrDefault();
                ProductVideoGoogleLists.Add(new ProductVideoGoogleList()
                {
                    @context = "https://schema.org/",
                    @type = "VideoObject",
                    name = "معرفی " + productVideo.Title,
                    description = productVideo.Abstract,
                    thumbnailUrl = new List<string>() { "https://www.tfshops.com/Content/UploadFiles/" + productVideo.VideoAttachmentCover.FileName },
                    uploadDate = string.Format("{0}T{1}+3:30", productVideo.VideoAttachment.InsertDate.ToString("yyyy-MM-dd"), productVideo.VideoAttachment.InsertDate.ToString("HH:mm:ss")),
                    duration = string.Format("PT{0}M{1}S", (productVideo.VideoAttachment.duration / 60).ToString(), (productVideo.VideoAttachment.duration % 60).ToString()),
                    contentUrl = "https://www.tfshops.com/Content/UploadFiles/" + productVideo.VideoAttachment.FileName,
                    embedUrl = "https://www.tfshops.com/embed/video/" + productVideo.Id,
                    interactionStatistic = new interactionStatistic()
                    {
                        @type = "InteractionCounter",
                        interactionType = new interactionType { type = "WatchAction" },
                        userInteractionCount = productVideo.Visits
                    }
                });
            }
            if (Any(x => x.Id, x => x.VideoOnbox.HasValue && x.VideoOnboxCover.HasValue && x.Id == productid))
            {
                var productVideo = GetQueryList().AsNoTracking().Include(x => x.VideoOnboxAttachment).Include(x => x.VideoOnboxAttachmentCover).Where(x => x.VideoOnbox.HasValue && x.VideoOnboxCover.HasValue && x.Id == productid).SingleOrDefault();
                ProductVideoGoogleLists.Add(new ProductVideoGoogleList()
                {
                    @context = "https://schema.org/",
                    @type = "VideoObject",
                    name = "آنباکس " + productVideo.Title,
                    description = productVideo.Abstract,
                    thumbnailUrl = new List<string>() { "https://www.tfshops.com/Content/UploadFiles/" + productVideo.VideoOnboxAttachmentCover.FileName },
                    uploadDate = string.Format("{0}T{1}+3:30", productVideo.VideoOnboxAttachment.InsertDate.ToString("yyyy-MM-dd"), productVideo.VideoOnboxAttachment.InsertDate.ToString("HH:mm:ss")),
                    duration = string.Format("PT{0}M{1}S", (productVideo.VideoOnboxAttachment.duration / 60).ToString(), (productVideo.VideoOnboxAttachment.duration % 60).ToString()),
                    contentUrl = "https://www.tfshops.com/Content/UploadFiles/" + productVideo.VideoOnboxAttachment.FileName,
                    embedUrl = "https://www.tfshops.com/embed/onbox/" + productVideo.Id,
                    interactionStatistic = new interactionStatistic()
                    {
                        @type = "InteractionCounter",
                        interactionType = new interactionType { type = "WatchAction" },
                        userInteractionCount = productVideo.Visits
                    }
                });
            }
            if (Any(x => x.Id, x => x.Audio.HasValue && x.AudioCover.HasValue && x.Id == productid))
            {
                if (Any(x => x.Id, x => x.AudioAttachment.FileName.Contains(".mp4") && x.Id == productid))
                {
                    var productVideo = GetQueryList().AsNoTracking().Include(x => x.AudioAttachment).Include(x => x.AudioAttachmentCover).Where(x => x.Audio.HasValue && x.AudioCover.HasValue && x.Id == productid).SingleOrDefault();
                    ProductVideoGoogleLists.Add(new ProductVideoGoogleList()
                    {
                        @context = "https://schema.org/",
                        @type = "VideoObject",
                        name = "پادکست " + productVideo.Title,
                        description = productVideo.Abstract,
                        thumbnailUrl = new List<string>() { "https://www.tfshops.com/Content/UploadFiles/" + productVideo.AudioAttachmentCover.FileName },
                        uploadDate = string.Format("{0}T{1}+3:30", productVideo.AudioAttachment.InsertDate.ToString("yyyy-MM-dd"), productVideo.AudioAttachment.InsertDate.ToString("HH:mm:ss")),
                        duration = string.Format("PT{0}M{1}S", (productVideo.AudioAttachment.duration / 60).ToString(), (productVideo.AudioAttachment.duration % 60).ToString()),
                        contentUrl = "https://www.tfshops.com/Content/UploadFiles/" + productVideo.AudioAttachment.FileName,
                        embedUrl = "https://www.tfshops.com/embed/podcast/" + productVideo.Id,
                        interactionStatistic = new interactionStatistic()
                        {
                            @type = "InteractionCounter",
                            interactionType = new interactionType { type = "WatchAction" },
                            userInteractionCount = productVideo.Visits
                        }
                    });
                }
            }

            return ProductVideoGoogleLists;
        }


        public string GetModeColorSize(string name, int? productModelId, ProductAttributeSelect productModel, int? productSizeId, ProductAttributeSelect productSize, string productSizeUnit, int? productColorId, ProductAttributeSelect productColor, int? productGarantyId, ProductAttributeSelect productGaranty, int? productWeightId, ProductAttributeSelect productWeight)
        {
            if (productModelId.HasValue)
                model = productModel.Value;
            else
                model = "";
            if (productSizeId.HasValue)
                size = productSize.Value + productSizeUnit;
            else
                size = "";
            if (productColorId.HasValue)
                color = " " + context.ProductAttributeItemColors.Find(int.Parse(context.ProductAttributeSelects.Find(productColorId).Value)).Color;
            else
                color = "";

            return model;
        }

        public bool CheckBuyer(int productId, string userid)
        {
            return Any(x => x.Id, x => x.OrderRows.Any(s => s.ProductId == productId) && x.OrderRows.Any(s => s.Order.UserId == userid) && x.OrderRows.Any(s => s.Order.OrderStates.Any(a => a.state == OrderStatus.تایید_پرداخت)) && x.OrderRows.Any(s => s.Order.OrderStates.Any(a => a.state == OrderStatus.تحویل_داده_شده)));
        }

        //public bool CheckProductPriceDefault(List<int> ids)
        //{
        //    try
        //    {
        //        var ProductPriceOrdersetting = context.Settings.Where(x => x.LanguageId == 1).Select(x => new { x.ProductPriceOrderFirst, x.ProductPriceOrderSecond, x.ProductPriceOrderThird, x.ProductPriceOrderfourth }).SingleOrDefault();

        //        foreach (var item in Get(x => x, x => ids.Contains(x.Id), null, "ProductPrices.Product,ProductPrices.ProductAttributeSelectModel,ProductPrices.ProductAttributeSelectSize,ProductPrices.ProductAttributeSelectColor"))
        //        {
        //            if (!item.ProductPrices.Any(x => x.IsDefault) || item.ProductPrices.Any(x => x.IsDefault && x.ProductStateId == 5))
        //            {
        //                #region setDefault
        //                foreach (var item2 in item.ProductPrices)
        //                {
        //                    item2.IsDefault = false;
        //                    //_productPriceService.Update(item2);
        //                }
        //                context.SaveChanges();

        //                var prps = item.ProductPrices.AsQueryable().OrderByDescending(x => x.Id);
        //                switch (ProductPriceOrdersetting.ProductPriceOrderFirst)
        //                {
        //                    case ProductPriceOrder.قیمت:
        //                        prps = prps.OrderBy(x => x.Price);
        //                        break;
        //                    case ProductPriceOrder.وضعیت_موجودی:
        //                        prps = prps.OrderBy(x => x.ProductStateId);
        //                        break;
        //                    case ProductPriceOrder.موجودی:
        //                        prps = prps.OrderByDescending(x => x.Quantity);
        //                        break;
        //                    case ProductPriceOrder.بازه_زمانی_ارسال:
        //                        prps = prps.OrderBy(x => x.DeliveryTimeout);
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                switch (ProductPriceOrdersetting.ProductPriceOrderSecond)
        //                {
        //                    case ProductPriceOrder.قیمت:
        //                        prps = prps.ThenBy(x => x.Price);
        //                        break;
        //                    case ProductPriceOrder.وضعیت_موجودی:
        //                        prps = prps.ThenBy(x => x.ProductStateId);
        //                        break;
        //                    case ProductPriceOrder.موجودی:
        //                        prps = prps.ThenByDescending(x => x.Quantity);
        //                        break;
        //                    case ProductPriceOrder.بازه_زمانی_ارسال:
        //                        prps = prps.ThenBy(x => x.DeliveryTimeout);
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                switch (ProductPriceOrdersetting.ProductPriceOrderThird)
        //                {
        //                    case ProductPriceOrder.قیمت:
        //                        prps = prps.ThenBy(x => x.Price);
        //                        break;
        //                    case ProductPriceOrder.وضعیت_موجودی:
        //                        prps = prps.ThenBy(x => x.ProductStateId);
        //                        break;
        //                    case ProductPriceOrder.موجودی:
        //                        prps = prps.ThenByDescending(x => x.Quantity);
        //                        break;
        //                    case ProductPriceOrder.بازه_زمانی_ارسال:
        //                        prps = prps.ThenBy(x => x.DeliveryTimeout);
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                switch (ProductPriceOrdersetting.ProductPriceOrderfourth)
        //                {
        //                    case ProductPriceOrder.قیمت:
        //                        prps = prps.ThenBy(x => x.Price);
        //                        break;
        //                    case ProductPriceOrder.وضعیت_موجودی:
        //                        prps = prps.ThenBy(x => x.ProductStateId);
        //                        break;
        //                    case ProductPriceOrder.موجودی:
        //                        prps = prps.ThenByDescending(x => x.Quantity);
        //                        break;
        //                    case ProductPriceOrder.بازه_زمانی_ارسال:
        //                        prps = prps.ThenBy(x => x.DeliveryTimeout);
        //                        break;
        //                    default:
        //                        break;
        //                }
        //                var defalutProductPrice = prps.First();
        //                defalutProductPrice.IsDefault = true;
        //                context.Entry(defalutProductPrice).State = EntityState.Modified;
        //                context.SaveChanges();

        //                string title = defalutProductPrice.Product.Name;
        //                string pageaddress = defalutProductPrice.Product.Name;
        //                if (defalutProductPrice.ProductAttributeSelectModelId.HasValue)
        //                {
        //                    title += " مدل " + defalutProductPrice.ProductAttributeSelectModel.Value;
        //                    pageaddress += " " + defalutProductPrice.ProductAttributeSelectModel.Value;
        //                }
        //                if (defalutProductPrice.ProductAttributeSelectSizeId.HasValue)
        //                    title += " سایز " + defalutProductPrice.ProductAttributeSelectSize.Value + defalutProductPrice.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit;
        //                if (defalutProductPrice.ProductAttributeSelectColorId.HasValue)
        //                    title += " " + context.ProductAttributeItemColors.Find(int.Parse(context.ProductAttributeSelects.Find(defalutProductPrice.ProductAttributeSelectColorId).Value)).Color;
        //                var product = defalutProductPrice.Product;
        //                product.PageAddress = CommonFunctions.NormalizeAddressWithSpace(pageaddress);
        //                product.Title = title;

        //                Update(product);
        //                //_productPriceService.Update(defalutProductPrice);
        //                context.SaveChanges();


        //                #endregion
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
    }

}
