using CoreLib;
using DataLayer;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class ProductPriceService : GenericRepository<ProductPrice>
    {
        private readonly ProductService _productService;
        public ProductPriceService(TfShopDbContext context, ProductService productService) : base(context)
        {
            this._productService = productService;

        }

        public ProductDetailItem ProductDetailItem(int productPriceId)
        {
            var q = GetQueryList().Include("Product.ProductComments.attachments").Include("ProductState").Include("ProductAttributeSelectModel").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectSize").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectweight").Include("Product.ProductPrices").Include("Product.ProductImages.Image").Include("ProductImages").Include("ProductImages.Image").Include("Product.Brand.attachment").Include("Product.ProductIcon").Include("ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductDisAdvantages").Include("Product.ProductAdvantages").Include("Product.VideoAttachment").Include("Product.ProductAttributeSelects").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttributeGroup").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttributeTab").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute.ProductAttributeItems").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute.ProductAttributeItemColors").Include("Product.ProductComments.ProductRankSelectValues").Include("Product.ProductCategories").Include("ProductAttributeSelectColor").Include("Product").Include("Product.ProductIcon").Include("Product.ProductRankSelects").Include("Product.ProductFavorates").Include("Product.ProductLetmeknows").Include("Product.ProductRankSelects.ProductRankSelectValues.ProductRankSelect.ProductRankGroupSelect.ProductRank").Include("Product.Tags").Include("Product.ProductQuestions").Include("Product.VideoOnboxAttachment").Include("Product.AudioAttachment").Include("Product.VideoOnboxAttachmentCover").Include("Product.AudioAttachmentCover").Include("Product.VideoAttachmentCover").Include("Product.VideoInstallationGuideAttachment").Include("Product.VideoInstallationGuideAttachmentCover").Include("Product.contentProducts.Content").Where(x => x.Id == productPriceId && x.IsActive);
            var result = q.ToList();


            return result.Select(x => new ProductDetailItem()
            {
                Id = x.Id,

                CancelFreeSend = x.Product.CancelFreeSend,
                ProductId = x.ProductId,
                Name = x.Product.Name,
                model = GetModeColorSize(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                size = _productService.size,
                color = _productService.color,
                LatinName = x.Product.LatinName,
                Title = x.IsDefault ? x.Product.Title : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                PageAddress = x.Product.PageAddress,
                Descr = x.Product.Descr,
                Code = x.code,
                ProductState = x.ProductState,
                ProductIcon = x.Product.ProductIcon,
                Brand = x.Product.Brand,
                MainImageFileName = x.ProductImages.Where(s => s.IsMain).Any() ? x.ProductImages.Where(s => s.IsMain).FirstOrDefault().Image.FileName : _productService.GetMainImageFileName(x.ProductId),
                OtherImages = x.ProductImages.Where(s => !s.IsMain),
                ProductOtherImages = x.Product.ProductImages.Where(s => !s.IsMain && !s.ProductPriceId.HasValue),
                UserOtherImages = x.Product.ProductComments.SelectMany(s => s.attachments),
                Price = _productService.GetPrice(x.ProductId, x.Price, x.Product.TaxId),
                finalPrice = _productService.cprice - _productService.GetOff(x.Id, x.Product.ProductCategories.FirstOrDefault().Id, x.Product.BrandId, _productService.cprice),
                offValue = x.ProductStateId < 3 ? _productService.coffvalue : 0,
                offFinalValue = x.ProductStateId < 3 ? _productService.coffvaluefinal : 0,
                tax = _productService.ctaxtvalue,
                offtype = _productService.cofftype,
                hasoff = x.ProductStateId < 3 ? _productService.chasoff : false,
                avgRankValue = x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? Math.Round((x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count()), 2) : 3,
                countRankValue = x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 1,
                countFAQ = x.Product.ProductQuestions.Count,
                Colors = x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Any() ? _productService.GetColorItems(x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Select(s => s.ProductAttributeSelectColor.Value)) : null,
                Garanties = x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty != null).Any() ? _productService.GetGarantyItems(x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty != null).Select(s => s.ProductAttributeSelectGaranty.Value)) : null,
                ProductCategories = x.Product.ProductCategories,
                Favorates = x.Product.ProductFavorates.Count,
                LetmeKnows = x.Product.ProductLetmeknows.Count,
                Abstract = x.Product.Abstract,
                Data = x.Product.Data,
                Data2 = x.Product.Data2,
                breadcrumb = _productService.GerProductBreadcrumb(x.ProductId),
                offTitle = _productService.offtitle,
                offExpireDate = _productService.offexpiredate,
                ProductPrices = x.Product.ProductPrices.Where(s => s.IsActive),
                ProductAttributeSelects = x.Product.ProductAttributeSelects,
                ProductRankSelectValues = x.Product.ProductComments.Any(s => s.IsActive) ? x.Product.ProductComments.Any(s => s.IsActive && s.ProductRankSelectValues.Any()) ? x.Product.ProductComments.Where(s => s.IsActive).SelectMany(s => s.ProductRankSelectValues) : null : null,
                //AdminProductRankSelectValues = x.Product.ProductRankSelects.Any(s => s.ProductRankSelectValues.Any(a => a.IsPrimary)) ? x.Product.ProductRankSelects.Where(s => s.ProductRankSelectValues.Any(a => a.IsPrimary)).SelectMany(s => s.ProductRankSelectValues.Where(a => a.IsPrimary)) : null,
                Video = x.Product.Video.HasValue ? x.Product.VideoAttachment.FileName : "",
                VideoOnbox = x.Product.VideoOnbox.HasValue ? x.Product.VideoOnboxAttachment.FileName : "",
                VideoInstallationGuide = x.Product.VideoInstallationGuide.HasValue ? x.Product.VideoInstallationGuideAttachment.FileName : "",
                VideoInstallationGuideId = x.Product.VideoInstallationGuide,
                VideoId = x.Product.Video,
                VideoOnboxId = x.Product.VideoOnbox,
                AudioId = x.Product.Audio,
                Audio = x.Product.Audio.HasValue ? x.Product.AudioAttachment.FileName : "",
                VideoCover = x.Product.VideoCover.HasValue ? x.Product.VideoAttachmentCover.FileName : "",
                AudioCover = x.Product.AudioCover.HasValue ? x.Product.AudioAttachmentCover.FileName : "",
                VideoOnboxCover = x.Product.VideoOnboxCover.HasValue ? x.Product.VideoOnboxAttachmentCover.FileName : "",
                VideoInstallationGuideCover = x.Product.VideoInstallationGuideCover.HasValue ? x.Product.VideoInstallationGuideAttachmentCover.FileName : "",
                ProductAdvantage = x.Product.ProductAdvantages.GroupBy(s => s.Title).Take(5).Select(s => s.Key),
                ProductCommentDisAdvantage = x.Product.ProductDisAdvantages.GroupBy(s => s.Title).Take(5).Select(s => s.Key),
                buyerattachments = x.Product.ProductComments.Any(s => s.IsActive) ? x.Product.ProductComments.Any(s => s.IsActive && s.ProductRankSelectValues.Any()) ? x.Product.ProductComments.Where(s => s.IsActive).SelectMany(s => s.attachments) : null : null,
                quantity = x.Quantity,
                Basketquantity = x.MaxBasketCount,
                productoffertype = _productService.ProductOfferType,
                offerQuantity = _productService.offerQuantity,
                offerMaxQuantity = _productService.offerMaxQuantity,
                Tags = x.Product.Tags,
                DeliveryTimeout = x.DeliveryTimeout,
                VideoTitle = x.Product.contentProducts.Any(a => a.contentProductType == contentProductType.معرفی_محصول) ? x.Product.contentProducts.Where(a => a.contentProductType == contentProductType.معرفی_محصول).First().Content.Title : "",
                VideoOnboxTitle = x.Product.contentProducts.Any(a => a.contentProductType == contentProductType.آنباکس_محصول) ? x.Product.contentProducts.Where(a => a.contentProductType == contentProductType.آنباکس_محصول).First().Content.Title : "",
                VideoInstallationGuideTitle = x.Product.contentProducts.Any(a => a.contentProductType == contentProductType.راهنمای_نصب_محصول) ? x.Product.contentProducts.Where(a => a.contentProductType == contentProductType.راهنمای_نصب_محصول).First().Content.Title : "",


            }).SingleOrDefault();
        }

        public ProductDetailItemV2 ProductDetailItemv2(int productPriceId)
        {
            var q = GetQueryList().Include("Product.ProductComments.attachments").Include("ProductState").Include("ProductAttributeSelectModel").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectSize").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectweight").Include("Product.ProductPrices").Include("Product.ProductImages.Image").Include("ProductImages").Include("ProductImages.Image").Include("Product.Brand.attachment").Include("Product.ProductIcon").Include("ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductDisAdvantages").Include("Product.ProductAdvantages").Include("Product.VideoAttachment").Include("Product.ProductAttributeSelects").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttributeGroup").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttributeTab").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute.ProductAttributeItems").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute.ProductAttributeItemColors").Include("Product.ProductComments.ProductRankSelectValues").Include("Product.ProductCategories").Include("ProductAttributeSelectColor").Include("Product").Include("Product.ProductIcon").Include("Product.ProductRankSelects").Include("Product.ProductFavorates").Include("Product.ProductLetmeknows").Include("Product.ProductRankSelects.ProductRankSelectValues.ProductRankSelect.ProductRankGroupSelect.ProductRank").Include("Product.Tags").Include("Product.ProductQuestions").Include("Product.VideoOnboxAttachment").Include("Product.AudioAttachment").Include("Product.VideoOnboxAttachmentCover").Include("Product.AudioAttachmentCover").Include("Product.VideoAttachmentCover").Include("Product.VideoInstallationGuideAttachment").Include("Product.VideoInstallationGuideAttachmentCover").Include("Product.contentProducts.Content").Where(x => x.Id == productPriceId && x.IsActive);
            var result = q.ToList();

            var prs = result.Select(x => new ProductDetailItemV2()
            {
                Id = x.Id,
                CancelFreeSend = x.Product.CancelFreeSend,
                ProductId = x.ProductId,
                Name = x.Product.Name,
                model = GetModeColorSizeV2(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                size = _productService.size,
                color = _productService.color,
                colortitle = _productService.colortitle,
                garanty = _productService.garanty,
                LatinName = x.Product.LatinName,
                Title = x.IsDefault ? x.Product.Title : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                PageAddress = x.Product.PageAddress.ToLower(),
                Code = x.code,
                ProductState = new productStateShortVM { id = x.ProductState.Id, name = x.ProductState.Title },
                Brand = new BrandShortVM { id = x.Product.Brand.Id, name = x.Product.Brand.Name, persianname = x.Product.Brand.PersianName, image = x.Product.Brand.AttachementId.HasValue ? x.Product.Brand.attachment.FileName : "default-thumbnail.jpg" },
                Price = _productService.GetPrice(x.ProductId, x.Price, x.Product.TaxId),
                finalPrice = _productService.cprice - _productService.GetOff(x.Id, x.Product.ProductCategories.FirstOrDefault().Id, x.Product.BrandId, _productService.cprice),
                offValue = x.ProductStateId < 3 ? _productService.coffvalue : 0,
                offFinalValue = x.ProductStateId < 3 ? _productService.coffvaluefinal : 0,
                tax = _productService.ctaxtvalue,
                offtype = _productService.cofftype,
                hasoff = x.ProductStateId < 3 ? _productService.chasoff : false,
                avgRankValue = x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? Math.Round((x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Sum(s => s.Value) / x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Count()), 2) : 3,
                countRankValue = x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).Any() ? x.Product.ProductRankSelects.SelectMany(s => s.ProductRankSelectValues).GroupBy(s => s.UserId).Count() : 1,
                countFAQ = x.Product.ProductQuestions.Count,
                Colors = x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Any() ? _productService.GetColorItems(x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectColorId != null && s.ProductAttributeSelectColor != null).Select(s => s.ProductAttributeSelectColor.Value)).Select(a => new productcolorShortVM() { id = a.Id, value = a.Value, color = a.Color }) : null,
                Garanties = x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty != null).Any() ? _productService.GetGarantyItems(x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty != null).Select(s => s.ProductAttributeSelectGaranty.Value)).Select(a => new productgarantyShortVM() { id = a.Id, value = a.Value, Descr = a.Descr }) : null,
                Sizes = x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectSize != null && s.ProductAttributeSelectSizeId != null).Any() ? x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectGarantyId != null && s.ProductAttributeSelectGaranty != null).Select(a => a.ProductAttributeSelectSize.Value.ToString()).Distinct() : null,
                Models = x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectModel != null && s.ProductAttributeSelectModelId != null).Any() ? x.Product.ProductPrices.Where(s => s.IsActive && s.ProductAttributeSelectModelId != null && s.ProductAttributeSelectModel != null).Select(a => a.ProductAttributeSelectModel.Value.ToString()).Distinct() : null,
                ProductCategory = new prcatShortVM { id = x.Product.ProductCategories.FirstOrDefault().Id, name = x.Product.ProductCategories.FirstOrDefault().Name, pageaddress = x.Product.ProductCategories.FirstOrDefault().PageAddress2.ToLower() },
                Favorates = x.Product.ProductFavorates.Count,
                LetmeKnows = x.Product.ProductLetmeknows.Count,
                offTitle = _productService.offtitle,
                offExpireDate = _productService.offexpiredate,
                ProductPrices = x.Product.ProductPrices.Where(s => s.IsActive).Select(a => new productpriceShortVM() { id = a.Id, ProductAttributeSelectColorId = a.ProductAttributeSelectColorId.HasValue ? a.ProductAttributeSelectColor.Value : "", ProductAttributeSelectGarantyId = x.ProductAttributeSelectGarantyId.HasValue ? x.ProductAttributeSelectGaranty.Value : "", ProductAttributeSelectModelId = a.ProductAttributeSelectModelId.HasValue ? a.ProductAttributeSelectModel.Value : "", ProductAttributeSelectSizeId = a.ProductAttributeSelectSizeId.HasValue ? a.ProductAttributeSelectSize.Value : "", ProductAttributeSelectWeightId = a.ProductAttributeSelectWeightId.HasValue ? a.ProductAttributeSelectweight.Value : "" }),
                quantity = x.Quantity,
                Basketquantity = x.MaxBasketCount,
                productoffertype = _productService.ProductOfferType,
                offerQuantity = _productService.offerQuantity,
                offerMaxQuantity = _productService.offerMaxQuantity,
                DeliveryTimeout = x.DeliveryTimeout,
                PrGallery = new PrGallery()
                {
                    Id = x.Id,
                    Title = x.Product.Title,
                    ProductId = x.ProductId,
                    MainImageFileName = x.ProductImages.Where(s => s.IsMain).Any() ? x.ProductImages.Where(s => s.IsMain).FirstOrDefault().Image.FileName : _productService.GetMainImageFileName(x.ProductId),
                    OtherImages = x.ProductImages.Where(s => !s.IsMain).Select(a => new productImageShortVM() { filename = a.Image.FileName, prname = a.Product.Name, prtitle = a.Product.Title }),
                    ProductOtherImages = x.Product.ProductImages.Where(s => !s.IsMain && !s.ProductPriceId.HasValue).Select(a => new productImageShortVM() { filename = a.Image.FileName, prname = a.Product.Name, prtitle = a.Product.Title }),
                    UserOtherImages = x.Product.ProductComments.SelectMany(s => s.attachments).Select(a => new productImageShortVM() { filename = a.FileName, prname = a.Title, prtitle = a.Title }),
                    Video = x.Product.Video.HasValue ? x.Product.VideoAttachment.FileName : "",
                    VideoOnbox = x.Product.VideoOnbox.HasValue ? x.Product.VideoOnboxAttachment.FileName : "",
                    VideoInstallationGuide = x.Product.VideoInstallationGuide.HasValue ? x.Product.VideoInstallationGuideAttachment.FileName : "",
                    VideoInstallationGuideId = x.Product.VideoInstallationGuide,
                    VideoId = x.Product.Video,
                    VideoOnboxId = x.Product.VideoOnbox,
                    AudioId = x.Product.Audio,
                    Audio = x.Product.Audio.HasValue ? x.Product.AudioAttachment.FileName : "",
                    VideoCover = x.Product.VideoCover.HasValue ? x.Product.VideoAttachmentCover.FileName : "",
                    AudioCover = x.Product.AudioCover.HasValue ? x.Product.AudioAttachmentCover.FileName : "",
                    VideoOnboxCover = x.Product.VideoOnboxCover.HasValue ? x.Product.VideoOnboxAttachmentCover.FileName : "",
                    VideoInstallationGuideCover = x.Product.VideoInstallationGuideCover.HasValue ? x.Product.VideoInstallationGuideAttachmentCover.FileName : "",
                    VideoTitle = x.Product.contentProducts.Any(a => a.contentProductType == contentProductType.معرفی_محصول) ? x.Product.contentProducts.Where(a => a.contentProductType == contentProductType.معرفی_محصول).First().Content.Title : x.Product.Title,
                    VideoOnboxTitle = x.Product.contentProducts.Any(a => a.contentProductType == contentProductType.آنباکس_محصول) ? x.Product.contentProducts.Where(a => a.contentProductType == contentProductType.آنباکس_محصول).First().Content.Title : x.Product.Title,
                    VideoInstallationGuideTitle = x.Product.contentProducts.Any(a => a.contentProductType == contentProductType.راهنمای_نصب_محصول) ? x.Product.contentProducts.Where(a => a.contentProductType == contentProductType.راهنمای_نصب_محصول).First().Content.Title : x.Product.Title,
                },
                PAttributeSelect = x.Product.ProductAttributeSelects.Select(a => new PAttributeSelect { Id = a.ProductAttributeGroupSelect.AttributeId, ProductId = a.ProductId, DisplayOrder = a.DisplayOrder, Title = a.ProductAttributeGroupSelect.ProductAttribute.Title, Value = a.Value }),
                Tags = x.Product.Tags.Select(a => new tagvm { Id = a.Id, Title = a.Title, TagName = a.TagName }),
                Data = x.Product.Data,
                ProductAdvantage = x.Product.ProductAdvantages.GroupBy(s => s.Title).Take(5).Select(s => s.Key),
                ProductCommentDisAdvantage = x.Product.ProductDisAdvantages.GroupBy(s => s.Title).Take(5).Select(s => s.Key),
                ProductRanks = x.Product.ProductComments.Any(s => s.IsActive) ? x.Product.ProductComments.Any(s => s.IsActive && s.ProductRankSelectValues.Any()) ? x.Product.ProductComments.Where(s => s.IsActive).SelectMany(s => s.ProductRankSelectValues).GroupBy(b => b.ProductRankSelect.ProductRankGroupSelect.ProductRank).Select(a => new keynameIdvm() { id = a.Key.Id, name = a.Key.Name }) : null : null,
                ProductRankSelectValues = x.Product.ProductComments.Any(s => s.IsActive) ? x.Product.ProductComments.Any(s => s.IsActive && s.ProductRankSelectValues.Any()) ? x.Product.ProductComments.Where(s => s.IsActive).SelectMany(s => s.ProductRankSelectValues).Select(a => new rankvm() { RankId = a.ProductRankSelect.ProductRankGroupSelect.RankId, value = a.Value }) : null : null,

            }).SingleOrDefault();
            if (prs == null)
                return null;
            var allCatIds = context.Database.SqlQuery<int>("exec GetParentCats @CatId", new SqlParameter("@CatId", prs.ProductCategory.id)).ToList();
            var ProductAttributeGroupSelects =
                context.ProductAttributeGroupProductCategory
                    .Include("ProductAttributeGroup.ProductAttributeGroupSelects.ProductAttribute.ProductAttributeItems")
                    .Where(c => allCatIds.Contains(c.ProductCategoryId))
                    .ToList()
                    .GroupBy(x => x.ProductAttributeGroupId)
                    .Select(g => g.First())
                    .Select(x => new ProductAttributeGroupSelectsvm
                    {
                        ProductAttributeGroupSelects =
                            x.ProductAttributeGroup.ProductAttributeGroupSelects
                                .Select(a => new groupAttsvm
                                {
                                    grouptitle = x.ProductAttributeGroup.Title,
                                    attname = string.IsNullOrEmpty(a.ProductAttribute.Title)
                                        ? a.ProductAttribute.Name
                                        : a.ProductAttribute.Title,
                                    attid = a.AttributeId,
                                    items = a.ProductAttribute.ProductAttributeItems
                                        .Select(b => new attItemvm
                                        {
                                            Id = b.Id,
                                            AttributeId = b.AttributeId,
                                            Value = b.Value
                                        }),
                                    DisplayGroupOrder = a.DisplayGroupOrder,
                                    DisplayOrder = a.DisplayOrder,
                                    Unit = a.ProductAttribute.Unit,
                                    DataType = a.ProductAttribute.DataType,
                                    Descr = a.ProductAttribute.Descr
                                })
                                .OrderBy(a => a.DisplayGroupOrder)
                                .ThenBy(a => a.DisplayOrder)
                                .AsEnumerable()
                    })
                    .AsEnumerable();

            prs.groups = ProductAttributeGroupSelects;

            return prs;
        }
        public IEnumerable<ProductDetailItem> ProductDetailItems(List<int> productPriceId)
        {
            var q = GetQueryList().Include("ProductState").Include("ProductAttributeSelectModel").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectSize").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectweight").Include("Product.ProductPrices").Include("Product.ProductImages.Image").Include("ProductImages").Include("ProductImages.Image").Include("Product.Brand").Include("Product.ProductIcon").Include("ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductAttributeSelects").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttributeGroup").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttributeTab").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute.ProductAttributeItems").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute.ProductAttributeItemColors").Include("Product.ProductComments.ProductRankSelectValues").Include("Product.ProductCategories").Include("ProductAttributeSelectColor").Include("Product").Include("Product.ProductIcon").Where(x => productPriceId.Contains(x.Id));
            var result = q.ToList();


            return result.Select(x => new ProductDetailItem()
            {
                Id = x.Id,
                ProductId = x.ProductId,
                Name = x.IsDefault ? x.Product.Name : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                LatinName = x.Product.LatinName,
                Title = x.IsDefault ? x.Product.Title : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                PageAddress = x.Product.PageAddress,
                Code = x.code,
                ProductState = x.ProductState,
                ProductIcon = x.Product.ProductIcon,
                MainImageFileName = x.ProductImages.Where(s => s.IsMain).Any() ? x.ProductImages.Where(s => s.IsMain).FirstOrDefault().Image.FileName : _productService.GetMainImageFileName(x.ProductId)

            });
        }

        public BasketShort ProductBasketItemShort(List<Basket> ProductBasketItems, int? UserCityId)
        {
            BasketShort basketShort = new BasketShort();
            List<int> productpriceids = ProductBasketItems.Select(s => s.ProductPriceId).ToList();


            var q = GetQueryList().AsNoTracking().Include("ProductState").Include("Product").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttributeGroup").Include("ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductCategories").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectWeight").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectModel").Include("ProductAttributeSelectSize").Where(x => productpriceids.Contains(x.Id) && x.IsActive);
            var result = q.ToList();

            var pboxes = GetBasketSendwayBoxesv2(ProductBasketItems);
            if (pboxes == null)
                return null;
            basketShort.ProductBasketItems = result.Select(x => new ProductBasketItem()
            {
                Id = x.Id,
                code = x.code,
                ProductId = x.ProductId,
                rawprice = x.Price,
                Name = x.IsDefault ? x.Product.Name : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                Title = x.IsDefault ? x.Product.Title : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                PageAddress = x.Product.PageAddress,
                Price = _productService.GetPrice(x.ProductId, x.Price, x.Product.TaxId),
                finalPrice = _productService.cprice - _productService.GetOff(x.Id, x.Product.ProductCategories.FirstOrDefault().Id, x.Product.BrandId, _productService.cprice),
                offValue = _productService.coffvalue,
                offFinalValue = _productService.coffvaluefinal,
                tax = _productService.ctaxtvalue,
                offtype = _productService.cofftype,
                hasoff = _productService.chasoff,
                offTitle = _productService.offtitle,
                offExpireDate = _productService.offexpiredate,
                offerid = _productService.offerId,
                //color = x.ProductAttributeSelectColorId.HasValue ? _productService.GetColorItem(int.Parse(x.ProductAttributeSelectColor.Value)) : null,
                //garanty = x.ProductAttributeSelectGarantyId.HasValue ? _productService.GetGarantyItem(int.Parse(x.ProductAttributeSelectGaranty.Value)) : null,
                //model = x.ProductAttributeSelectModelId.HasValue ? x.ProductAttributeSelectModel.Value : "",
                //size = x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.Value : "",
                //weight = x.ProductAttributeSelectWeightId.HasValue ? x.ProductAttributeSelectweight.Value : "",
                //productWeight = x.Product.ProductWeight,
                //SenwayBoxId = pboxes.Where(s => s.ProductPriceIdList.Any(a => a == x.Id)).First().SendwayBox.Id,
                //PacakgeType = pboxes.Where(s => s.ProductPriceIdList.Any(a => a == x.Id)).First().SendwayBox.ProductPackageType.EnumDisplayNameFor(),
                ProductPackageType = pboxes.Where(s => s.ProductPriceIdList.Any(a => a == x.Id)).First().SendwayBox.ProductPackageType,
                MainImageFileName = _productService.GetMainProductPriceImageFileName(x.Id, x.ProductId),
                UserQuantity = ProductBasketItems.Where(w => w.ProductPriceId == x.Id).FirstOrDefault().Quantity,
                ProductPriceQuantity = x.MaxBasketCount > x.Quantity ? x.Quantity : x.MaxBasketCount,
                offerQuantity = _productService.offerQuantity,
                offerMaxQuantity = _productService.offerMaxQuantity,
                //extrarice = x.Product.ExtraSendPrice * ProductBasketItems.Where(w => w.ProductPriceId == x.Id).FirstOrDefault().Quantity,
                TaxId = x.Product.TaxId,
                DeliveryTimeout = x.DeliveryTimeout,
                InsertDate = ProductBasketItems.Where(w => w.ProductPriceId == x.Id).FirstOrDefault() != null ? ProductBasketItems.Where(w => w.ProductPriceId == x.Id).FirstOrDefault().InsertDate : null
                //Bon = x.Product.bon,
                //MaxBon = x.Product.bonMaxBasket,
                //productStateId = x.ProductStateId.Value

            }).OrderByDescending(x => x.InsertDate);

            if (UserCityId.HasValue)
                basketShort.remainPrice = CheckFreeSendway(ProductBasketItems.Select(x => x.ProductPriceId).ToList(), UserCityId.Value, basketShort.ProductBasketItems.Sum(x => x.finalPrice * x.UserQuantity), basketShort.ProductBasketItems.Select(x => x.ProductPackageType).ToList(), 0);
            else
                basketShort.remainPrice = -1;

            return basketShort;
        }
        public IEnumerable<ProductBasketItem> ProductBasketItem(List<Basket> ProductBasketItems)
        {
            List<int> productpriceids = ProductBasketItems.Select(s => s.ProductPriceId).ToList();


            var q = GetQueryList().AsNoTracking().Include("ProductState").Include("Product").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttributeGroup").Include("ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductCategories").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectWeight").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectModel").Include("ProductAttributeSelectSize").Where(x => productpriceids.Contains(x.Id) && x.IsActive);
            var result = q.ToList();


            var pboxes = GetBasketSendwayBoxesv2(ProductBasketItems);
            if (pboxes == null)
                return null;
            return result.Select(x => new ProductBasketItem()
            {
                Id = x.Id,
                DeliveryTimeout = x.DeliveryTimeout,
                CancelInPlacePayment = x.Product.CancelInPlacePayment,
                code = x.code,
                ProductId = x.ProductId,
                rawprice = x.Price,
                Name = x.IsDefault ? x.Product.Name : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                Title = x.IsDefault ? x.Product.Title : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                PageAddress = x.Product.PageAddress,
                Price = _productService.GetPrice(x.ProductId, x.Price, x.Product.TaxId),
                finalPrice = _productService.cprice - _productService.GetOff(x.Id, x.Product.ProductCategories.FirstOrDefault().Id, x.Product.BrandId, _productService.cprice),
                offValue = _productService.coffvalue,
                offFinalValue = _productService.coffvaluefinal,
                tax = _productService.ctaxtvalue,
                offtype = _productService.cofftype,
                hasoff = _productService.chasoff,
                offTitle = _productService.offtitle,
                offExpireDate = _productService.offexpiredate,
                offerid = _productService.offerId,
                color = x.ProductAttributeSelectColorId.HasValue ? _productService.GetColorItem(int.Parse(x.ProductAttributeSelectColor.Value)) : null,
                garanty = x.ProductAttributeSelectGarantyId.HasValue ? _productService.GetGarantyItem(int.Parse(x.ProductAttributeSelectGaranty.Value)) : null,
                // model = x.ProductAttributeSelectModelId.HasValue ? x.ProductAttributeSelectModel.Value : "",
                // size = x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.Value : "",
                //weight = x.ProductAttributeSelectWeightId.HasValue ? x.ProductAttributeSelectweight.Value : "",
                productWeight = x.Product.ProductWeight,
                SenwayBoxId = pboxes.Where(s => s.ProductPriceIdList.Any(a => a == x.Id)).First().SendwayBox.Id,
                PacakgeType = pboxes.Where(s => s.ProductPriceIdList.Any(a => a == x.Id)).First().SendwayBox.ProductPackageType.EnumDisplayNameFor(),
                ProductPackageType = pboxes.Where(s => s.ProductPriceIdList.Any(a => a == x.Id)).First().SendwayBox.ProductPackageType,
                MainImageFileName = _productService.GetMainProductPriceImageFileName(x.Id, x.ProductId),
                UserQuantity = ProductBasketItems.Where(w => w.ProductPriceId == x.Id).FirstOrDefault().Quantity,
                ProductPriceQuantity = x.MaxBasketCount > x.Quantity ? x.Quantity : x.MaxBasketCount,
                offerQuantity = _productService.offerQuantity,
                offerMaxQuantity = _productService.offerMaxQuantity,
                //extraprice = x.Product.ExtraSendPrice * ProductBasketItems.Where(w => w.ProductPriceId == x.Id).FirstOrDefault().Quantity,
                extraprice = x.Product.ExtraSendPrice,
                TaxId = x.Product.TaxId,
                Bon = x.Product.bon,
                MaxBon = x.Product.bonMaxBasket,
                productStateId = x.ProductStateId.Value,
                CatId = x.Product.ProductCategories.First().Id,
                we = x.Product.ProductWeight,
                l = x.Product.Lenght,
                h = x.Product.Height,
                w = x.Product.Width
            });
        }

        public IEnumerable<ProductBasketItemV2> ProductBasketItemV2(List<Basket> ProductBasketItems)
        {
            List<int> productpriceids = ProductBasketItems.Select(s => s.ProductPriceId).ToList();


            var q = GetQueryList().AsNoTracking().Include("ProductState").Include("Product").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttributeGroup").Include("ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductAttributeSelects.ProductAttributeGroupSelect.ProductAttribute").Include("Product.ProductCategories").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectWeight").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectModel").Include("ProductAttributeSelectSize").Where(x => productpriceids.Contains(x.Id) && x.IsActive);
            var result = q.ToList();


            var pboxes = GetBasketSendwayBoxesv2(ProductBasketItems);
            if (pboxes == null)
                return null;
            return result.Select(x => new ProductBasketItemV2()
            {
                Id = x.Id,
                DeliveryTimeout = x.DeliveryTimeout,
                CancelInPlacePayment = x.Product.CancelInPlacePayment,
                code = x.code,
                ProductId = x.ProductId,
                rawprice = x.Price,
                Name = x.IsDefault ? x.Product.Name : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                Title = x.IsDefault ? x.Product.Title : GetTitle(x.Product.Name, x.ProductAttributeSelectModelId, x.ProductAttributeSelectModel, x.ProductAttributeSelectSizeId, x.ProductAttributeSelectSize, x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.ProductAttributeGroupSelect.ProductAttribute.Unit : "", x.ProductAttributeSelectColorId, x.ProductAttributeSelectColor, x.ProductAttributeSelectGarantyId, x.ProductAttributeSelectGaranty, x.ProductAttributeSelectWeightId, x.ProductAttributeSelectweight),
                PageAddress = x.Product.PageAddress,
                Price = _productService.GetPrice(x.ProductId, x.Price, x.Product.TaxId),
                finalPrice = _productService.cprice - _productService.GetOff(x.Id, x.Product.ProductCategories.FirstOrDefault().Id, x.Product.BrandId, _productService.cprice),
                offValue = _productService.coffvalue,
                offFinalValue = _productService.coffvaluefinal,
                tax = _productService.ctaxtvalue,
                offtype = _productService.cofftype,
                hasoff = _productService.chasoff,
                offTitle = _productService.offtitle,
                offExpireDate = _productService.offexpiredate,
                offerid = _productService.offerId,
                color = x.ProductAttributeSelectColorId.HasValue ? _productService.GetColorItem(int.Parse(x.ProductAttributeSelectColor.Value)).Value: null,
                colorcode= x.ProductAttributeSelectColorId.HasValue ? _productService.GetColorItem(int.Parse(x.ProductAttributeSelectColor.Value)).Color : null,
                garanty = x.ProductAttributeSelectGarantyId.HasValue ? _productService.GetGarantyItem(int.Parse(x.ProductAttributeSelectGaranty.Value)).Value : null,
                // model = x.ProductAttributeSelectModelId.HasValue ? x.ProductAttributeSelectModel.Value : "",
                // size = x.ProductAttributeSelectSizeId.HasValue ? x.ProductAttributeSelectSize.Value : "",
                //weight = x.ProductAttributeSelectWeightId.HasValue ? x.ProductAttributeSelectweight.Value : "",
                productWeight = x.Product.ProductWeight,
                SenwayBoxId = pboxes.Where(s => s.ProductPriceIdList.Any(a => a == x.Id)).First().SendwayBox.Id,
                PacakgeType = pboxes.Where(s => s.ProductPriceIdList.Any(a => a == x.Id)).First().SendwayBox.ProductPackageType.EnumDisplayNameFor(),
                ProductPackageType = Convert.ToInt32(pboxes.Where(s => s.ProductPriceIdList.Any(a => a == x.Id)).First().SendwayBox.ProductPackageType),
                MainImageFileName = _productService.GetMainProductPriceImageFileName(x.Id, x.ProductId),
                UserQuantity = ProductBasketItems.Where(w => w.ProductPriceId == x.Id).FirstOrDefault().Quantity,
                ProductPriceQuantity = x.MaxBasketCount > x.Quantity ? x.Quantity : x.MaxBasketCount,
                offerQuantity = _productService.offerQuantity,
                offerMaxQuantity = _productService.offerMaxQuantity,
                //extraprice = x.Product.ExtraSendPrice * ProductBasketItems.Where(w => w.ProductPriceId == x.Id).FirstOrDefault().Quantity,
                extraprice = x.Product.ExtraSendPrice,
                TaxId = x.Product.TaxId,
                Bon = x.Product.bon,
                MaxBon = x.Product.bonMaxBasket,
                productStateId = x.ProductStateId.Value,
                CatId = x.Product.ProductCategories.First().Id,
                we = x.Product.ProductWeight,
                l = x.Product.Lenght,
                h = x.Product.Height,
                w = x.Product.Width
            });
        }


        /// <summary>
        /// لیست ماهیت محموله های محصولات
        /// </summary>
        /// <param name="productIdList"></param>
        /// <returns></returns>
        public List<ProductBox> GetBasketSendwayBoxes(List<Basket> products)
        {
            List<ProductBox> productBoxes = new List<ProductBox>();
            try
            {
                List<int> ProductPriceIds = products.Select(x => x.ProductPriceId).ToList();
                // به دست آوردن مشخصات حجمی محصولات موجود در سبد
                var productList = context.ProductPrices.Where(x => ProductPriceIds.Contains(x.Id)).Select(x => new { x.Id, x.ProductId, x.Product.ProductWeight, x.Product.Lenght, x.Product.Width, x.Product.Height });
                // به دست آوردن بزرگترین طول، عرض یا ارتفاع محصولات موجود در سبد
                int MaxSize = productList.Max(x => x.Lenght);
                if (productList.Max(x => x.Width) > MaxSize)
                    MaxSize = productList.Max(x => x.Width);
                if (productList.Max(x => x.Height) > MaxSize)
                    MaxSize = productList.Max(x => x.Height);
                // به دست آوردن بزرگترین محصول
                var maxProduct = productList.Where(x => x.Lenght == MaxSize || x.Width == MaxSize || x.Height == MaxSize).First();
                // به دست آوردن کوچکترین باکس قابل انتخاب نسبت به بزرگترین عدد 
                var sendwayBox = context.SendwayBoxes.Where(x => x.ProductSendWayBoxes.Any(s => s.ProductSendWay.ProductSendWaySelects.Any(a => a.ProductId == maxProduct.ProductId)));
                //باکس های روش های ارسالِ انتخابی محصول
                if (sendwayBox.Any())
                {
                    //باکس های ارسالی با توجه به ابعاد و وزن و همینطور باکسهای روش های ارسالی انتخابی محصول
                    List<SendwayBox> ProductSendWayBoxes = GetProductSendWayBoxes(MaxSize, maxProduct.ProductWeight);
                    if (ProductSendWayBoxes.Any())
                    {
                        List<int> ProductSendWayBoxesId = ProductSendWayBoxes.Select(x => x.Id).ToList();
                        sendwayBox = sendwayBox.Where(x => ProductSendWayBoxesId.Contains(x.Id));
                        // انتخاب بزرگترین باکس فروشگاه
                        if (!sendwayBox.Any())
                        {
                            int MaxBoxId = context.SendwayBoxes.OrderByDescending(x => x.Lenght * x.Width * x.Height).ThenByDescending(x => x.Lenght).ThenByDescending(x => x.Width).ThenByDescending(x => x.Height).ThenByDescending(x => x.ProductWeight).First().Id;
                            sendwayBox = sendwayBox.Where(x => x.Id == MaxBoxId);
                        }
                    }
                    // انتخاب بزرگترین باکس فروشگاه
                    else
                    {
                        int MaxBoxId = context.SendwayBoxes.OrderByDescending(x => x.Lenght * x.Width * x.Height).ThenByDescending(x => x.Lenght).ThenByDescending(x => x.Width).ThenByDescending(x => x.Height).ThenByDescending(x => x.ProductWeight).First().Id;
                        sendwayBox = sendwayBox.Where(x => x.Id == MaxBoxId);
                    }
                }
                // انتخاب بزرگترین باکس فروشگاه
                else
                {

                    int MaxBoxId = context.SendwayBoxes.OrderByDescending(x => x.Lenght * x.Width * x.Height).ThenByDescending(x => x.Lenght).ThenByDescending(x => x.Width).ThenByDescending(x => x.Height).ThenByDescending(x => x.ProductWeight).First().Id;
                    sendwayBox = sendwayBox.Where(x => x.Id == MaxBoxId);
                }


                // کوچکترین باکس قابل انتخاب برای بزرگترین محصول
                var finalSendwaybox = sendwayBox.OrderBy(x => x.Lenght * x.Width * x.Height).ThenBy(x => x.Height).ThenBy(x => x.Width).ThenBy(x => x.Lenght).ThenBy(x => x.ProductWeight).First();
                //تخصیص بزرگترین محصول به باکس انتخابی
                productBoxes.Add(new ProductBox() { SendwayBox = finalSendwaybox, ProductPriceIdList = new List<int>() { maxProduct.Id } });
                List<int> solvedBoxProducts = productBoxes.SelectMany(x => x.ProductPriceIdList).ToList();
                // انتخاب باکس برای بقیه محصولات
                foreach (var item in productList.Where(x => x.Id != maxProduct.Id).OrderBy(x => (x.Lenght * x.Width * x.Height)))
                {
                    int Quantity = products.Where(x => x.ProductPriceId == item.Id).First().Quantity;
                    //آیا محصول جاری در بزرگترین باکس جا می شود؟
                    List<int> fpids = productBoxes.SelectMany(a => a.ProductPriceIdList).ToList();
                    var filledProductList = context.ProductPrices.Where(x => fpids.Contains(x.Id)).Select(x => new { x.Id, x.ProductId, x.Product.ProductWeight, x.Product.Lenght, x.Product.Width, x.Product.Height });
                    int filledBoxMass = 0;
                    foreach (var filledPr in filledProductList)
                    {
                        filledBoxMass += (filledPr.Width * filledPr.Height * filledPr.Lenght) * products.Where(x => x.ProductPriceId == filledPr.Id).First().Quantity;
                    }
                    int itemMass = (item.Width * item.Height * item.Lenght * Quantity);
                    int finalSendwayboxMass = (finalSendwaybox.Width * finalSendwaybox.Height * finalSendwaybox.Lenght) - (filledBoxMass);
                    if (finalSendwayboxMass > itemMass)
                    //if (finalSendwayboxMass > itemMass && finalSendwaybox.ProductPackageType == ProductPackageType.normal)
                    {
                        productBoxes.Where(x => x.SendwayBox == finalSendwaybox).First().ProductPriceIdList.Add(item.Id);
                    }
                    // جا نمیشود و باید باکس دیگری بیابیم
                    else
                    {
                        // به دست آوردن کوچکترین باکس قابل انتخاب برای محصول جاری
                        var itemSendwayBox = context.SendwayBoxes.Where(x => x.ProductSendWayBoxes.Any(s => s.ProductSendWay.ProductSendWaySelects.Any(a => a.ProductId == item.ProductId)));
                        //باکس های روش های ارسالِ انتخابی محصول
                        if (itemSendwayBox.Any())
                        {
                            //باکس های ارسالی با توجه به حجم و وزن و همینطور باکسهای روش های ارسالی انتخابی محصول
                            List<SendwayBox> ProductSendWayBoxes = GetProductSendWayBoxesWithMass(item.Width * Quantity, item.Height * Quantity, item.Lenght * Quantity, item.ProductWeight * Quantity);
                            if (ProductSendWayBoxes.Any())
                            {
                                List<int> ProductSendWayBoxesId = ProductSendWayBoxes.Select(x => x.Id).ToList();
                                itemSendwayBox = itemSendwayBox.Where(x => ProductSendWayBoxesId.Contains(x.Id));

                                if (productBoxes.Any(x => x.SendwayBox == itemSendwayBox))
                                {

                                    productBoxes.Where(x => x.SendwayBox == itemSendwayBox).First().ProductPriceIdList.Add(item.Id);
                                }
                                else
                                    productBoxes.Add(new ProductBox() { SendwayBox = itemSendwayBox.First(), ProductPriceIdList = new List<int>() { item.Id } });

                            }
                            // انتخاب بزرگترین باکس بعدی و بررسی ماهیت و در صورت لزوم انتقال محصولات به باکس جدید
                            else
                            {
                                var productList2 = productList.Where(x => !solvedBoxProducts.Contains(x.Id));
                                int MaxSize2 = productList2.Max(x => x.Lenght);
                                if (productList2.Max(x => x.Width) > MaxSize2)
                                    MaxSize2 = productList2.Max(x => x.Width);
                                if (productList2.Max(x => x.Height) > MaxSize2)
                                    MaxSize2 = productList2.Max(x => x.Height);
                                // به دست آوردن بزرگترین محصول
                                var maxProduct2 = productList2.Where(x => !solvedBoxProducts.Contains(x.Id) && (x.Lenght == MaxSize2 || x.Width == MaxSize2 || x.Height == MaxSize2)).First();
                                // به دست آوردن کوچکترین باکس قابل انتخاب نسبت به بزرگترین عدد 
                                var sendwayBox2 = context.SendwayBoxes.Where(x => x.ProductSendWayBoxes.Any(s => s.ProductSendWay.ProductSendWaySelects.Any(a => a.ProductId == maxProduct2.ProductId)));
                                if (sendwayBox2.Any())
                                {
                                    //باکس های ارسالی با توجه به ابعاد و وزن و همینطور باکسهای روش های ارسالی انتخابی محصول
                                    List<SendwayBox> ProductSendWayBoxes2 = GetProductSendWayBoxes(MaxSize2, maxProduct2.ProductWeight);
                                    if (ProductSendWayBoxes2.Any())
                                    {
                                        List<int> ProductSendWayBoxesId = ProductSendWayBoxes2.Select(x => x.Id).ToList();
                                        sendwayBox2 = sendwayBox2.Where(x => ProductSendWayBoxesId.Contains(x.Id));
                                        // انتخاب بزرگترین باکس فروشگاه
                                        if (!sendwayBox2.Any())
                                        {
                                            int MaxBoxId = context.SendwayBoxes.OrderByDescending(x => x.Lenght * x.Width * x.Height).ThenByDescending(x => x.Lenght).ThenByDescending(x => x.Width).ThenByDescending(x => x.Height).ThenByDescending(x => x.ProductWeight).First().Id;
                                            sendwayBox2 = sendwayBox2.Where(x => x.Id == MaxBoxId);
                                        }
                                    }
                                    // انتخاب بزرگترین باکس فروشگاه
                                    else
                                    {
                                        int MaxBoxId = context.SendwayBoxes.OrderByDescending(x => x.Lenght * x.Width * x.Height).ThenByDescending(x => x.Lenght).ThenByDescending(x => x.Width).ThenByDescending(x => x.Height).ThenByDescending(x => x.ProductWeight).First().Id;
                                        sendwayBox2 = sendwayBox2.Where(x => x.Id == MaxBoxId);
                                    }
                                }
                                // انتخاب بزرگترین باکس فروشگاه
                                else
                                {

                                    int MaxBoxId = context.SendwayBoxes.OrderByDescending(x => x.Lenght * x.Width * x.Height).ThenByDescending(x => x.Lenght).ThenByDescending(x => x.Width).ThenByDescending(x => x.Height).ThenByDescending(x => x.ProductWeight).First().Id;
                                    sendwayBox2 = sendwayBox.Where(x => x.Id == MaxBoxId);
                                }


                                // کوچکترین باکس قابل انتخاب برای بزرگترین محصول
                                var finalSendwaybox2 = sendwayBox2.OrderBy(x => x.Lenght * x.Width * x.Height).ThenBy(x => x.Height).ThenBy(x => x.Width).ThenBy(x => x.Lenght).ThenBy(x => x.ProductWeight).First();
                                //تخصیص محصول به باکس انتخابی
                                productBoxes.Add(new ProductBox() { SendwayBox = finalSendwaybox2, ProductPriceIdList = new List<int>() { item.Id } });
                                // آیا ماهیت باکس با بزرگترین باکس یکی است؟
                                if (finalSendwaybox.ProductPackageType == finalSendwaybox2.ProductPackageType)
                                {
                                    //انتقال به باکس جدید
                                    var oldBox = productBoxes.Where(x => x.SendwayBox == finalSendwaybox).ToList();
                                    productBoxes.RemoveAll(x => x.SendwayBox == finalSendwaybox);
                                    foreach (var pbox in oldBox)
                                    {
                                        foreach (var id in pbox.ProductPriceIdList)
                                        {

                                            productBoxes.Where(x => x.SendwayBox == finalSendwaybox2).First().ProductPriceIdList.Add(id);
                                        }
                                    }
                                }
                            }
                        }
                        // انتخاب بزرگترین باکس فروشگاه
                        else
                        {

                            int MaxBoxId = context.SendwayBoxes.OrderByDescending(x => x.Lenght * x.Width * x.Height).ThenByDescending(x => x.Lenght).ThenByDescending(x => x.Width).ThenByDescending(x => x.Height).ThenByDescending(x => x.ProductWeight).First().Id;
                            itemSendwayBox = itemSendwayBox.Where(x => x.Id == MaxBoxId);
                            if (productBoxes.Any(x => x.SendwayBox == itemSendwayBox))
                            {

                                productBoxes.Where(x => x.SendwayBox == itemSendwayBox).First().ProductPriceIdList.Add(item.Id);
                            }
                            else
                                productBoxes.Add(new ProductBox() { SendwayBox = itemSendwayBox.First(), ProductPriceIdList = new List<int>() { item.Id } });
                        }

                    }

                    if (productBoxes.GroupBy(x => x.SendwayBox).Select(x => x.Key).Count() > 1)
                    {
                        var maxpacakge = productBoxes.Max(x => x.SendwayBox.ProductPackageType);
                        var maxbox = productBoxes.Where(x => x.SendwayBox.ProductPackageType == maxpacakge).OrderByDescending(x => x.SendwayBox.Lenght * x.SendwayBox.Width * x.SendwayBox.Height).Select(x => x.SendwayBox).FirstOrDefault();
                        productBoxes.ForEach(x => x.SendwayBox = maxbox);
                    }
                    solvedBoxProducts = productBoxes.SelectMany(x => x.ProductPriceIdList).ToList();
                }



                //_productService.packagename = finalSendwaybox.ProductPackageType.EnumDisplayNameFor();
                //_productService.ProductPackageType = finalSendwaybox.ProductPackageType;

                //return finalSendwaybox.Id;
            }
            catch (Exception ex)
            {
                context.EventLogs.Add(new EventLog()
                {
                    ActionName = "GetBasketSendwayBoxes",
                    ControllerName = "Cart",
                    Description = ex.Message + ex.InnerException != null ? ex.InnerException.Message : "",
                    LogDateTime = DateTime.Now,
                    LogType = 5,
                    RequestType = false,
                    StatusCode = 500,
                    UserId = context.Users.First().Id

                });
                context.SaveChanges();
            }
            return productBoxes;

        }


        public List<ProductBox> GetBasketSendwayBoxesv2(List<Basket> products)
        {
            List<ProductBox> productBoxes = new List<ProductBox>();
            try
            {
                List<int> ProductPriceIds = products.Select(x => x.ProductPriceId).ToList();
                List<int> ProductIds = products.Select(x => x.ProductId).ToList();
                List<int> productsendwayid = context.ProductSendWaySelects.AsQueryable().AsNoTracking().Where(x => ProductIds.Contains(x.ProductId)).Select(x => x.ProductSendWayId).ToList();
                // به دست آوردن مشخصات حجمی محصولات موجود در سبد
                var productList = context.ProductPrices.Where(x => ProductPriceIds.Contains(x.Id)).Select(x => new { x.Id, x.ProductId, capacity = x.Product.Lenght * x.Product.Width * x.Product.Height, x.Product.ProductWeight, x.Product.Width, x.Product.Lenght }).ToList();
                int sumCapacity = 0, sumWeight = 0, maxWidth = 0, maxLenght = 0;
                foreach (var item in productList)
                {
                    sumCapacity += item.capacity * products.Where(a => a.ProductPriceId == item.Id).Single().Quantity;
                    sumWeight += item.ProductWeight * products.Where(a => a.ProductPriceId == item.Id).Single().Quantity;
                    if (item.Width > maxWidth)
                        maxWidth = item.Width;
                    if (item.Lenght > maxLenght)
                        maxLenght = item.Lenght;
                }
                var selectedBox = context.SendwayBoxes.Where(x => x.ProductSendWayBoxes.Any(a => productsendwayid.Contains(a.ProductSendWayId)) && (x.Width * x.Height * x.Lenght >= sumCapacity) && (x.ProductWeight >= sumWeight) && (x.Width >= maxWidth && x.Lenght >= maxLenght)).OrderBy(x => x.Width * x.Height * x.Lenght).FirstOrDefault();
                if (selectedBox != null)
                {
                    foreach (var item in productList)
                    {
                        productBoxes.Add(new ProductBox() { SendwayBox = selectedBox, ProductPriceIdList = new List<int>() { item.Id } });
                    }
                }
                else
                    return null;


            }
            catch (Exception ex)
            {
                context.EventLogs.Add(new EventLog()
                {
                    ActionName = "GetBasketSendwayBoxes",
                    ControllerName = "Cart",
                    Description = ex.Message + ex.InnerException != null ? ex.InnerException.Message : "",
                    LogDateTime = DateTime.Now,
                    LogType = 5,
                    RequestType = false,
                    StatusCode = 500,
                    UserId = context.Users.First().Id

                });
                context.SaveChanges();
            }
            return productBoxes;

        }
        public List<ProductBoxV2> GetBasketSendwayBoxesv22(List<Basket> products)
        {
            List<ProductBoxV2> productBoxes = new List<ProductBoxV2>();
            try
            {
                List<int> ProductPriceIds = products.Select(x => x.ProductPriceId).ToList();
                List<int> ProductIds = products.Select(x => x.ProductId).ToList();
                List<int> productsendwayid = context.ProductSendWaySelects.AsQueryable().AsNoTracking().Where(x => ProductIds.Contains(x.ProductId)).Select(x => x.ProductSendWayId).ToList();
                // به دست آوردن مشخصات حجمی محصولات موجود در سبد
                var productList = context.ProductPrices.Where(x => ProductPriceIds.Contains(x.Id)).Select(x => new { x.Id, x.ProductId, capacity = x.Product.Lenght * x.Product.Width * x.Product.Height, x.Product.ProductWeight, x.Product.Width, x.Product.Lenght }).ToList();
                int sumCapacity = 0, sumWeight = 0, maxWidth = 0, maxLenght = 0;
                foreach (var item in productList)
                {
                    sumCapacity += item.capacity * products.Where(a => a.ProductPriceId == item.Id).Single().Quantity;
                    sumWeight += item.ProductWeight * products.Where(a => a.ProductPriceId == item.Id).Single().Quantity;
                    if (item.Width > maxWidth)
                        maxWidth = item.Width;
                    if (item.Lenght > maxLenght)
                        maxLenght = item.Lenght;
                }
                var selectedBox = context.SendwayBoxes.Where(x => x.ProductSendWayBoxes.Any(a => productsendwayid.Contains(a.ProductSendWayId)) && (x.Width * x.Height * x.Lenght >= sumCapacity) && (x.ProductWeight >= sumWeight) && (x.Width >= maxWidth && x.Lenght >= maxLenght)).OrderBy(x => x.Width * x.Height * x.Lenght).FirstOrDefault();
                if (selectedBox != null)
                {
                    foreach (var item in productList)
                    {
                        productBoxes.Add(new ProductBoxV2() { SendwayBox = new SendwayBoxVM() { Id = selectedBox.Id, Description = selectedBox.Description, Height = selectedBox.Height, Lenght = selectedBox.Lenght, ProductPackageType = Convert.ToInt32(selectedBox.ProductPackageType), ProductWeight = selectedBox.ProductWeight, Title = selectedBox.Title, Width = selectedBox.Width }, ProductPriceIdList = new List<int>() { item.Id } });
                    }
                }
                else
                    return null;


            }
            catch (Exception ex)
            {
                context.EventLogs.Add(new EventLog()
                {
                    ActionName = "GetBasketSendwayBoxes",
                    ControllerName = "Cart",
                    Description = ex.Message + ex.InnerException != null ? ex.InnerException.Message : "",
                    LogDateTime = DateTime.Now,
                    LogType = 5,
                    RequestType = false,
                    StatusCode = 500,
                    UserId = context.Users.First().Id

                });
                context.SaveChanges();
            }
            return productBoxes;

        }

        /// <summary>
        /// لیست باکس های قابل ارسال محصول از روی بزرگترین اندازه
        /// </summary>
        /// <param name="w">عرض</param>
        /// <param name="h">ارتفاع</param>
        /// <param name="l">طول</param>
        /// <param name="we">وزن</param>
        /// <returns></returns>
        public List<SendwayBox> GetProductSendWayBoxes(int max, int we)
        {

            var sendwayBox = context.SendwayBoxes.AsQueryable();
            sendwayBox = sendwayBox.Where(x => (x.Lenght > max || x.Width > max || x.Height > max) && x.ProductWeight > we);
            return sendwayBox.ToList();

        }

        /// <summary>
        /// لیست باکس های قابل ارسال محصول از روی حجم
        /// </summary>
        /// <param name="w">عرض</param>
        /// <param name="h">ارتفاع</param>
        /// <param name="l">طول</param>
        /// <param name="we">وزن</param>
        /// <returns></returns>
        public List<SendwayBox> GetProductSendWayBoxesWithMass(int w, int h, int l, int we)
        {
            int mass = w * h * l;
            var sendwayBox = context.SendwayBoxes.AsQueryable();
            sendwayBox = sendwayBox.Where(x => (x.Lenght * x.Width * x.Height) > mass && x.ProductWeight > we);
            return sendwayBox.ToList();

        }
        public string GetTitle(string name, int? productModelId, ProductAttributeSelect productModel, int? productSizeId, ProductAttributeSelect productSize, string productSizeUnit, int? productColorId, ProductAttributeSelect productColor, int? productGarantyId, ProductAttributeSelect productGaranty, int? productWeightId, ProductAttributeSelect productWeight)
        {
            string title = name;
            if (productModelId.HasValue)
            {
                title += " مدل " + productModel.Value;
                _productService.model = productModel.Value;
            }
            else
                _productService.model = "";
            if (productSizeId.HasValue)
            {
                title += " سایز " + productSize.Value + productSizeUnit;
                _productService.size = productSize.Value + productSizeUnit;
            }
            else
                _productService.size = "";
            if (productColorId.HasValue)
            {
                _productService.color = context.ProductAttributeItemColors.Find(int.Parse(context.ProductAttributeSelects.Find(productColorId).Value)).Color;
                title += " " + _productService.color;
            }
            else
                _productService.color = "";

            return title;
        }
        public string GetModeColorSize(string name, int? productModelId, ProductAttributeSelect productModel, int? productSizeId, ProductAttributeSelect productSize, string productSizeUnit, int? productColorId, ProductAttributeSelect productColor, int? productGarantyId, ProductAttributeSelect productGaranty, int? productWeightId, ProductAttributeSelect productWeight)
        {
            if (productModelId.HasValue)
                _productService.model = productModel.Value;
            else
                _productService.model = "";
            if (productSizeId.HasValue)
                _productService.size = productSize.Value + productSizeUnit;
            else
                _productService.size = "";
            if (productColorId.HasValue)
                _productService.color = " " + context.ProductAttributeItemColors.Find(int.Parse(context.ProductAttributeSelects.Find(productColorId).Value)).Color;
            else
                _productService.color = "";

            return _productService.model;
        }
        public string GetModeColorSizeV2(string name, int? productModelId, ProductAttributeSelect productModel, int? productSizeId, ProductAttributeSelect productSize, string productSizeUnit, int? productColorId, ProductAttributeSelect productColor, int? productGarantyId, ProductAttributeSelect productGaranty, int? productWeightId, ProductAttributeSelect productWeight)
        {
            if (productModelId.HasValue)
                _productService.model = productModel.Value;
            else
                _productService.model = "";
            if (productSizeId.HasValue)
                _productService.size = productSize.Value;
            else
                _productService.size = "";
            if (productColorId.HasValue)
            {
                _productService.color = productColor.Value;
                _productService.colortitle = context.ProductAttributeItemColors.Find(int.Parse(context.ProductAttributeSelects.Find(productColorId).Value)).Color;
            }
            else
            {
                _productService.color = "";
                _productService.colortitle = "";
            }
            if (productGarantyId.HasValue)
                _productService.garanty = productGaranty.Value;
            else
                _productService.garanty = "";

            return _productService.model;
        }

        /// <summary>
        /// برگرداندن تنوع از روی مدل،سایز،رنگ و یا گارانتی
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="productId"></param>
        /// <param name="model"></param>
        /// <param name="size"></param>
        /// <param name="color"></param>
        /// <param name="garanty"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public ProductPrice GetProductPrice(string selector, int productId, string model, string size, int? color, int? garanty, string weight)
        {
            model = model.ToLower();
            size = size.ToLower();

            string itemcolorid = "";
            if (color.HasValue)
                itemcolorid = color.Value.ToString();
            string itemid = "";
            if (garanty.HasValue)
                itemid = garanty.Value.ToString();

            var prp = context.ProductPrices.Include("ProductAttributeSelectModel").Include("ProductAttributeSelectColor").Include("ProductAttributeSelectSize").Include("ProductAttributeSelectGaranty").Include("ProductAttributeSelectweight").Include("Product").AsNoTracking().AsQueryable();
            prp = prp.Where(x => x.ProductId == productId);
            if (!String.IsNullOrEmpty(model))
                prp = prp.Where(x => x.ProductAttributeSelectModel.Value.ToLower() == model);
            if (!String.IsNullOrEmpty(size))
                prp = prp.Where(x => x.ProductAttributeSelectSize.Value.ToLower() == size);
            if (color.HasValue)
                prp = prp.Where(x => x.ProductAttributeSelectColor.Value == itemcolorid);
            if (garanty.HasValue)
                prp = prp.Where(x => x.ProductAttributeSelectGaranty.Value == itemid);
            if (!String.IsNullOrEmpty(weight))
                prp = prp.Where(x => x.ProductAttributeSelectweight.Value == weight);
            if (prp.ToList().Any())
            {
                var productprice = prp.FirstOrDefault();
                return productprice;
            }
            else
            {
                switch (selector)
                {
                    case "Models":
                        return Get(x => x, x => x.ProductId == productId && x.ProductAttributeSelectModel.Value.ToLower() == model, null, "ProductAttributeSelectModel,ProductAttributeSelectColor,ProductAttributeSelectSize,ProductAttributeSelectGaranty,ProductAttributeSelectweight,Product").First();
                    case "Sizes":
                        return Get(x => x, x => x.ProductId == productId && x.ProductAttributeSelectSize.Value.ToLower() == size, null, "ProductAttributeSelectModel,ProductAttributeSelectColor,ProductAttributeSelectSize,ProductAttributeSelectGaranty,ProductAttributeSelectweight,Product").First();
                    case "Colors":
                        return Get(x => x, x => x.ProductId == productId && x.ProductAttributeSelectColor.Value == itemcolorid, null, "ProductAttributeSelectModel,ProductAttributeSelectColor,ProductAttributeSelectSize,ProductAttributeSelectGaranty,ProductAttributeSelectweight,Product").First();
                    case "Garanties":
                        return Get(x => x, x => x.ProductId == productId && x.ProductAttributeSelectGaranty.Value == itemid, null, "ProductAttributeSelectModel,ProductAttributeSelectColor,ProductAttributeSelectSize,ProductAttributeSelectGaranty,ProductAttributeSelectweight,Product").First();
                    default:
                        return Get(x => x, x => x.ProductId == productId && x.ProductAttributeSelectColor.Value == itemcolorid, null, "ProductAttributeSelectModel,ProductAttributeSelectColor,ProductAttributeSelectSize,ProductAttributeSelectGaranty,ProductAttributeSelectweight,Product").First();
                }
            }


        }

        public int? GetSendwayCost(List<int> ProductPriceIds, int userCityid, int sendwayboxid, int sendwayid, double weight, long OrderSumPrice, int extraPrice, out bool freesend, List<ProductPackageType> productPackageTypes = null)
        {
            try
            {
                freesend = false;
                int cost = 0;
                var sendway = context.ProductSendWays.SingleOrDefault(x => x.Id == sendwayid);

                #region ارسال رایگان
                if (sendway.FreeOff)
                {
                    //در محصولات انتخابی محصولی وجود ندارد که عدم ارسال رایگان در مرسوله باشد؟
                    if (!context.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id) && a.Product.CancelFreeSend == true))
                    {
                        List<int> catIds = context.ProductPrices.Where(x => x.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id))).SelectMany(x => x.Product.ProductCategories).Select(x => x.Id).ToList();
                        List<int> brands = context.ProductPrices.Where(x => x.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id))).Select(x => x.Product.BrandId.Value).ToList();
                        List<int> allCatIds = new List<int>();
                        foreach (var item in catIds)
                        {
                            allCatIds.AddRange(context.Database.SqlQuery<int>("exec GetParentCats @CatId", new SqlParameter("@CatId", item)).ToList());
                        }
                        //محصولات انتخابی در شهر کاربر ارسال رایگان شده اند؟
                        var freeSendOffer = context.FreeSendOffers.Where(s => s.Offer.IsDeleted == false && s.FreeSendOfferStates.Any(x => x.CityId == userCityid && x.SendwayBoxId == sendwayboxid) && (s.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id)) || brands.Contains(s.BrandId.Value) || allCatIds.Contains(s.CatId.Value)) && s.Offer.IsDeleted == false && s.Offer.IsDeleted == false && s.Offer.IsActive && s.Offer.state == true && ((s.Offer.ExpireDate != null && s.Offer.ExpireDate >= DateTime.Now) || s.Offer.ExpireDate == null) && ((s.Offer.StartDate != null && s.Offer.StartDate <= DateTime.Now) || s.Offer.StartDate == null));
                        //var freeSendOffer = context.FreeSendOffers.Where(s => s.FreeSendOfferStates.Any(x => x.CityId == userCityid) && (s.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id)) || brands.Contains(s.BrandId.Value) || allCatIds.Contains(s.CatId.Value)) );
                        //سقف ارسال رایگان رعایت شده است؟
                        bool valid = true, valid2 = true;
                        if (freeSendOffer.Any())
                        {
                            if (productPackageTypes != null)
                            {
                                foreach (var item in productPackageTypes)
                                {

                                    if (!context.ProductSendWayBoxes.Any(x => x.ProductSendWay.FreeOff && x.SendwayBox.ProductPackageType == item))
                                        valid2 = false;
                                }
                            }

                            foreach (var item in freeSendOffer)
                            {
                                if (OrderSumPrice < item.OrderSum)
                                {
                                    valid = false;
                                    break;
                                }
                            }

                            if (valid)
                            {
                                ////محاسبه هزینه ارسال با پست برای بررسی سقف مجاز هزینه ارسال
                                //var box = context.SendwayBoxes.Find(sendwayboxid);
                                //var boxWeight = (box.Lenght * box.Height * box.Width) / 6;
                                //cost = IrPostCalculation(userCityid, boxWeight > weight ? boxWeight : weight) + extraPrice;

                                //محاسبه هزینه ارسال با پست برای بررسی سقف مجاز هزینه ارسال
                                //cost = IrPostCalculation(userCityid, weight) + extraPrice;

                                //foreach (var item in freeSendOffer)
                                //{
                                //    if (cost > item.MaxCost)
                                //    {
                                //        valid = false;
                                //        break;
                                //    }
                                //}

                            }
                            if (valid && valid2)
                            {
                                cost = 0;
                                freesend = true;
                            }
                            else
                                return -1;

                        }
                    }

                    return cost;
                }
                #endregion

                #region پست ایران
                if (sendway.IsIrPost)
                {
                    if (context.ProductSendwayIrPostDetails.Where(x => x.ProductSendWayId == sendwayid).Any())
                    {
                        //هزینه
                        if (sendway.IsFree)
                        {
                            freesend = true;
                            cost = 0;
                        }
                        else
                        {
                            var box = context.SendwayBoxes.Find(sendwayboxid);
                            var boxWeight = (box.Lenght * box.Height * box.Width) / 6;
                            cost = IrPostCalculation(userCityid, boxWeight > weight ? boxWeight : weight);

                            //cost = cost + (cost * Convert.ToInt32(Math.Ceiling(context.SendwayBoxes.Find(sendwayboxid).BoxPricePercent / 100.0)));
                        }


                        return cost;
                    }
                    else
                    {
                        return null;
                    }
                }
                #endregion

                #region پیک و پس کرایه
                else
                {
                    if (sendway.PasKeraye)
                    {
                        cost = 0;
                        return cost;
                    }
                    else
                    {
                        if (context.ProductSendWayDetails.Where(x => x.ProductSendWayBox.SendWayBoxID == sendwayboxid && x.ProductSendWayBox.ProductSendWayId == sendwayid && x.CityId == userCityid && x.IsActive).Any())
                        {
                            //هزینه ارزان ترین
                            cost = context.ProductSendWayDetails.Where(x => x.ProductSendWayBox.SendWayBoxID == sendwayboxid && x.ProductSendWayBox.ProductSendWayId == sendwayid && x.CityId == userCityid && x.IsActive).OrderBy(s => s.Value).First().Value;
                            if (sendway.IsFree)
                            {
                                freesend = true;
                                cost = 0;
                            }
                            else
                            {
                                if (sendway.TaxId.HasValue)
                                {
                                    cost = cost + Convert.ToInt32(Math.Ceiling((cost * (context.Taxes.Find(sendway.TaxId).TaxPercent / 100.0))));
                                }

                            }
                            return cost;
                        }
                        else
                            return null;
                    }

                }
                #endregion

                //else
                //{

                //    return 0;
                //}
            }
            catch (Exception ex)
            {
                context.EventLogs.Add(new EventLog()
                {
                    ActionName = "GetSendwayCost",
                    ControllerName = "Cart",
                    Description = ex.Message + ex.InnerException != null ? ex.InnerException.Message : "",
                    LogDateTime = DateTime.Now,
                    LogType = 5,
                    RequestType = false,
                    StatusCode = 500,
                    UserId = context.Users.First().Id

                });
                context.SaveChanges();

                freesend = false;
                return null;
            }

        }
        public int? GetSendwayCostV2(List<int> ProductPriceIds, int userCityid, int sendwayboxid, int sendwayid, double weight, long OrderSumPrice, int extraPrice, out bool freesend, List<int> productPackageTypes = null)
        {
            try
            {
                freesend = false;
                int cost = 0;
                var sendway = context.ProductSendWays.SingleOrDefault(x => x.Id == sendwayid);

                #region ارسال رایگان
                if (sendway.FreeOff)
                {
                    //در محصولات انتخابی محصولی وجود ندارد که عدم ارسال رایگان در مرسوله باشد؟
                    if (!context.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id) && a.Product.CancelFreeSend == true))
                    {
                        List<int> catIds = context.ProductPrices.Where(x => x.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id))).SelectMany(x => x.Product.ProductCategories).Select(x => x.Id).ToList();
                        List<int> brands = context.ProductPrices.Where(x => x.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id))).Select(x => x.Product.BrandId.Value).ToList();
                        List<int> allCatIds = new List<int>();
                        foreach (var item in catIds)
                        {
                            allCatIds.AddRange(context.Database.SqlQuery<int>("exec GetParentCats @CatId", new SqlParameter("@CatId", item)).ToList());
                        }
                        //محصولات انتخابی در شهر کاربر ارسال رایگان شده اند؟
                        var freeSendOffer = context.FreeSendOffers.Where(s => s.Offer.IsDeleted == false && s.FreeSendOfferStates.Any(x => x.CityId == userCityid && x.SendwayBoxId == sendwayboxid) && (s.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id)) || brands.Contains(s.BrandId.Value) || allCatIds.Contains(s.CatId.Value)) && s.Offer.IsDeleted == false && s.Offer.IsDeleted == false && s.Offer.IsActive && s.Offer.state == true && ((s.Offer.ExpireDate != null && s.Offer.ExpireDate >= DateTime.Now) || s.Offer.ExpireDate == null) && ((s.Offer.StartDate != null && s.Offer.StartDate <= DateTime.Now) || s.Offer.StartDate == null));
                        //var freeSendOffer = context.FreeSendOffers.Where(s => s.FreeSendOfferStates.Any(x => x.CityId == userCityid) && (s.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id)) || brands.Contains(s.BrandId.Value) || allCatIds.Contains(s.CatId.Value)) );
                        //سقف ارسال رایگان رعایت شده است؟
                        bool valid = true, valid2 = true;
                        if (freeSendOffer.Any())
                        {
                            if (productPackageTypes != null)
                            {
                                ProductPackageType? prpkt = null;
                                foreach (var item in productPackageTypes)
                                {
                                    prpkt = (ProductPackageType)item;
                                    if (!context.ProductSendWayBoxes.Any(x => x.ProductSendWay.FreeOff && x.SendwayBox.ProductPackageType == prpkt))
                                        valid2 = false;
                                }
                            }

                            foreach (var item in freeSendOffer)
                            {
                                if (OrderSumPrice < item.OrderSum)
                                {
                                    valid = false;
                                    break;
                                }
                            }

                            if (valid)
                            {
                                ////محاسبه هزینه ارسال با پست برای بررسی سقف مجاز هزینه ارسال
                                //var box = context.SendwayBoxes.Find(sendwayboxid);
                                //var boxWeight = (box.Lenght * box.Height * box.Width) / 6;
                                //cost = IrPostCalculation(userCityid, boxWeight > weight ? boxWeight : weight) + extraPrice;

                                //محاسبه هزینه ارسال با پست برای بررسی سقف مجاز هزینه ارسال
                                //cost = IrPostCalculation(userCityid, weight) + extraPrice;

                                //foreach (var item in freeSendOffer)
                                //{
                                //    if (cost > item.MaxCost)
                                //    {
                                //        valid = false;
                                //        break;
                                //    }
                                //}

                            }
                            if (valid && valid2)
                            {
                                cost = 0;
                                freesend = true;
                            }
                            else
                                return -1;

                        }
                    }

                    return cost;
                }
                #endregion

                #region پست ایران
                if (sendway.IsIrPost)
                {
                    if (context.ProductSendwayIrPostDetails.Where(x => x.ProductSendWayId == sendwayid).Any())
                    {
                        //هزینه
                        if (sendway.IsFree)
                        {
                            freesend = true;
                            cost = 0;
                        }
                        else
                        {
                            var box = context.SendwayBoxes.Find(sendwayboxid);
                            var boxWeight = (box.Lenght * box.Height * box.Width) / 6;
                            cost = IrPostCalculation(userCityid, boxWeight > weight ? boxWeight : weight);

                            //cost = cost + (cost * Convert.ToInt32(Math.Ceiling(context.SendwayBoxes.Find(sendwayboxid).BoxPricePercent / 100.0)));
                        }


                        return cost;
                    }
                    else
                    {
                        return null;
                    }
                }
                #endregion

                #region پیک و پس کرایه
                else
                {
                    if (sendway.PasKeraye)
                    {
                        cost = 0;
                        return cost;
                    }
                    else
                    {
                        if (context.ProductSendWayDetails.Where(x => x.ProductSendWayBox.SendWayBoxID == sendwayboxid && x.ProductSendWayBox.ProductSendWayId == sendwayid && x.CityId == userCityid && x.IsActive).Any())
                        {
                            //هزینه ارزان ترین
                            cost = context.ProductSendWayDetails.Where(x => x.ProductSendWayBox.SendWayBoxID == sendwayboxid && x.ProductSendWayBox.ProductSendWayId == sendwayid && x.CityId == userCityid && x.IsActive).OrderBy(s => s.Value).First().Value;
                            if (sendway.IsFree)
                            {
                                freesend = true;
                                cost = 0;
                            }
                            else
                            {
                                if (sendway.TaxId.HasValue)
                                {
                                    cost = cost + Convert.ToInt32(Math.Ceiling((cost * (context.Taxes.Find(sendway.TaxId).TaxPercent / 100.0))));
                                }

                            }
                            return cost;
                        }
                        else
                            return null;
                    }

                }
                #endregion

                //else
                //{

                //    return 0;
                //}
            }
            catch (Exception ex)
            {
                context.EventLogs.Add(new EventLog()
                {
                    ActionName = "GetSendwayCost",
                    ControllerName = "Cart",
                    Description = ex.Message + ex.InnerException != null ? ex.InnerException.Message : "",
                    LogDateTime = DateTime.Now,
                    LogType = 5,
                    RequestType = false,
                    StatusCode = 500,
                    UserId = context.Users.First().Id

                });
                context.SaveChanges();

                freesend = false;
                return null;
            }

        }

        private int IrPostCalculation(int userCityid, double weight)
        {
            int cost = 0;
            int userProvienceId = context.Cities.Where(x => x.Id == userCityid).First().ProvinceId;
            int? shoppingProvienceId = context.Settings.Where(x => x.LanguageId == 1).First().ProvinceId;
            if (!shoppingProvienceId.HasValue)
                shoppingProvienceId = 8;
            var ProductSendwayIrPostDetail = context.ProductSendwayIrPostDetails.First();
            statePosition statePosition = statePosition.درون_استانی;
            if (userProvienceId != shoppingProvienceId)
            {
                if (context.SettingStates.Any(x => x.ProvinceId == userProvienceId))
                    statePosition = statePosition.برون_استانی_همجوار;
                else
                    statePosition = statePosition.برون_استانی_غیر_همجوار;
            }
            if (statePosition == statePosition.درون_استانی && weight <= 1000)
                cost = context.ProductSendwayIrPostDetails.First().InnserState1;
            else if (statePosition == statePosition.درون_استانی && weight > 1000)
                cost = Convert.ToInt32(ProductSendwayIrPostDetail.InnserState1 + Math.Ceiling(((weight - 1000) / 1000.0) * ProductSendwayIrPostDetail.InnserStateOver1));

            else if (statePosition == statePosition.برون_استانی_همجوار && weight <= 1000)
                cost = context.ProductSendwayIrPostDetails.First().OuterNearState1;
            else if (statePosition == statePosition.برون_استانی_همجوار && weight > 1000)
                cost = Convert.ToInt32(ProductSendwayIrPostDetail.OuterNearState1 + Math.Ceiling(((weight - 1000) / 1000.0) * ProductSendwayIrPostDetail.OuterNearStateOver1));

            else if (statePosition == statePosition.برون_استانی_غیر_همجوار && weight <= 1000)
                cost = context.ProductSendwayIrPostDetails.First().OuterState1;
            else if (statePosition == statePosition.برون_استانی_غیر_همجوار && weight > 1000)
                cost = Convert.ToInt32(ProductSendwayIrPostDetail.OuterState1 + Math.Ceiling(((weight - 1000) / 1000.0) * ProductSendwayIrPostDetail.OuterStateOver1));
            cost = Convert.ToInt32(Math.Ceiling(cost / 1000.0) * 1000);
            return cost;
        }

        public long CheckFreeSendway(List<int> ProductPriceIds, int userCityid, long OrderSumPrice, List<ProductPackageType> productPackageTypes, int boxid)
        {
            long remainCost = 0;
            long allofferprice = 0;
            var sendway = context.ProductSendWays.Where(x => x.FreeOff).FirstOrDefault();

            #region ارسال رایگان
            if (sendway != null)
            {
                //در محصولات انتخابی محصولی وجود ندارد که عدم ارسال رایگان در مرسوله باشد؟
                if (!context.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id) && a.Product.CancelFreeSend == true))
                {
                    List<int> catIds = context.ProductPrices.Where(x => x.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id))).SelectMany(x => x.Product.ProductCategories).Select(x => x.Id).ToList();
                    List<int> brands = context.ProductPrices.Where(x => x.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id))).Select(x => x.Product.BrandId.Value).ToList();
                    List<int> allCatIds = new List<int>();
                    foreach (var item in catIds)
                    {
                        allCatIds.AddRange(context.Database.SqlQuery<int>("exec GetParentCats @CatId", new SqlParameter("@CatId", item)).ToList());
                    }
                    //محصولات انتخابی در شهر کاربر ارسال رایگان شده اند؟
                    var freeSendOffer = context.FreeSendOffers.Where(s => s.Offer.IsDeleted == false && s.FreeSendOfferStates.Any(x => x.CityId == userCityid && x.SendwayBoxId == boxid) && (s.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id)) || brands.Contains(s.BrandId.Value) || allCatIds.Contains(s.CatId.Value)) && s.Offer.IsActive && s.Offer.IsDeleted == false && s.Offer.state == true && ((s.Offer.ExpireDate != null && s.Offer.ExpireDate >= DateTime.Now) || s.Offer.ExpireDate == null) && ((s.Offer.StartDate != null && s.Offer.StartDate <= DateTime.Now) || s.Offer.StartDate == null)).ToList();
                    //var freeSendOffer = context.FreeSendOffers.Where(s => s.FreeSendOfferStates.Any(x => x.CityId == userCityid) && (s.Product.ProductPrices.Any(a => ProductPriceIds.Contains(a.Id)) || brands.Contains(s.BrandId.Value) || allCatIds.Contains(s.CatId.Value)) );
                    //سقف ارسال رایگان رعایت شده است؟
                    bool valid = true, valid2 = true;
                    if (freeSendOffer.Any())
                    {
                        foreach (var item in productPackageTypes)
                        {

                            if (!context.ProductSendWayBoxes.Any(x => x.ProductSendWay.FreeOff && x.SendwayBox.ProductPackageType == item))
                                valid2 = false;
                        }

                        foreach (var item in freeSendOffer)
                        {
                            if (OrderSumPrice < item.OrderSum)
                            {
                                valid = false;
                                remainCost = item.OrderSum.Value - OrderSumPrice;
                                allofferprice += item.OrderSum.Value;
                            }
                        }
                        if (valid && valid2)
                            return 0;
                        else
                        {
                            if (remainCost <= allofferprice * 0.3)
                                return remainCost;
                            else
                                return -1;
                        }



                    }
                    else
                        return -1;
                }
                else
                    return -1;
            }
            else
                return -1;
            #endregion


        }

        public int CorrectQuantity(int userQuantity, int MaxQuantity, int Quantity, int offerQuantity, int offerMaxQuantity)
        {
            if (offerQuantity > 0 && offerMaxQuantity > 0)
            {
                int max = 0;
                if (offerMaxQuantity < offerQuantity)
                    max = offerMaxQuantity;
                else
                    max = offerQuantity;

                if (userQuantity <= max) // تعداد وارد شده از موجودی و حداکثر قابل انتخاب کوچکتر و مساوی می باشد
                    return userQuantity;
                else
                    return max;
            }
            else
            {
                if (userQuantity <= Quantity && userQuantity <= MaxQuantity) // تعداد وارد شده از موجودی و حداکثر قابل انتخاب کوچکتر و مساوی می باشد
                    return userQuantity;
                else
                {
                    int max = MaxQuantity;
                    if (MaxQuantity > Quantity)
                        max = Quantity;
                    return max;
                }
            }
        }
    }
    public enum statePosition
    {
        درون_استانی,
        برون_استانی_همجوار,
        برون_استانی_غیر_همجوار
    }

}
