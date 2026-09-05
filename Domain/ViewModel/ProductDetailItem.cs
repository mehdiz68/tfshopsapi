using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;

namespace Domain.ViewModels
{
    public class ProductDetailItem
    {
        #region Ctor
        public ProductDetailItem()
        {

        }
        #endregion

        #region Properties
        
        public int Id { get; set; }
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string model { get; set; }
        public string size { get; set; }
        public string color { get; set; }


        public string LatinName { get; set; }

        public string Title { get; set; }


        public string PageAddress { get; set; }

        public string Descr { get; set; }
        public string Data { get; set; }
        public string Data2 { get; set; }
        public string Abstract { get; set; }

        public int quantity { get; set; }
        public int Basketquantity { get; set; }
        public Brand Brand { get; set; }

        public int DeliveryTimeout { get; set; }

        public string Code { get; set; }

        public bool CancelFreeSend { get; set; }
        public  ProductState ProductState { get; set; }

        public  ProductIcon ProductIcon { get; set; }

        public string MainImageFileName { get; set; }
        public ProductImage MainImage { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<ProductImage> OtherImages{ get; set; }
        public IEnumerable<ProductImage> ProductOtherImages { get; set; }
        public IEnumerable<attachment> UserOtherImages { get; set; }
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<ProductPrice> ProductPrices { get; set; }
        public IEnumerable<ProductAttributeSelect> ProductAttributeSelects { get; set; }
        public IEnumerable<ProductAttributeItemColor> Colors { get; set; }
        public IEnumerable<ProductAttributeItem> Garanties { get; set; }
        public IEnumerable<ProductRankSelectValue> ProductRankSelectValues { get; set; }
        public IEnumerable<ProductRankSelectValue> AdminProductRankSelectValues { get; set; }
        public IEnumerable<attachment> buyerattachments { get; set; }

        public double avgRankValue { get; set; }
        public int countRankValue { get; set; }
        public int countFAQ{ get; set; }

        public long Price { get; set; }
        public long finalPrice { get; set; }
        public long offValue{ get; set; }
        public long offFinalValue { get; set; }
        public bool productoffertype { get; set; }
        public int offerQuantity { get; set; }
        public int offerMaxQuantity { get; set; }

        public double tax { get; set; }

        public bool hasoff { get; set; }

        public string offTitle { get; set; }

        public string offExpireDate{ get; set; }
        public short offtype { get; set; }
        public int Favorates { get; set; }
        public int LetmeKnows { get; set; }
        public string breadcrumb { get; set; }

        public Guid? VideoId { get; set; }
        public string Video { get; set; }
        public string VideoCover { get; set; }
        public string VideoTitle { get; set; }
        public string VideoOnbox { get; set; }
        public Guid? VideoOnboxId { get; set; }
        public string VideoOnboxTitle { get; set; }
        public string VideoOnboxCover { get; set; }
        public Guid? VideoInstallationGuideId { get; set; }
        public string VideoInstallationGuide { get; set; }
        public string VideoInstallationGuideTitle { get; set; }
        public string VideoInstallationGuideCover { get; set; }
        public Guid? AudioId { get; set; }
        public string Audio { get; set; }
        public string AudioCover { get; set; }
        public IEnumerable<string> ProductAdvantage { get; set; }
        public IEnumerable<string> ProductCommentDisAdvantage { get; set; }
        #endregion
    }

    public class ProductDetailItemV2
    {
        #region Ctor
        public ProductDetailItemV2()
        {

        }
        #endregion

        #region Properties

        public int Id { get; set; }
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string model { get; set; }
        public string size { get; set; }
        public string garanty { get; set; }
        public string color { get; set; }
        public string colortitle { get; set; }


        public string LatinName { get; set; }

        public string Title { get; set; }


        public string PageAddress { get; set; }


        public int quantity { get; set; }
        public int Basketquantity { get; set; }

        public int DeliveryTimeout { get; set; }

        public string Code { get; set; }

        public bool CancelFreeSend { get; set; }

        public double avgRankValue { get; set; }
        public int countRankValue { get; set; }
        public int countFAQ { get; set; }

        public long Price { get; set; }
        public long finalPrice { get; set; }
        public long offValue { get; set; }
        public long offFinalValue { get; set; }
        public bool productoffertype { get; set; }
        public int offerQuantity { get; set; }
        public int offerMaxQuantity { get; set; }

        public double tax { get; set; }

        public bool hasoff { get; set; }

        public string offTitle { get; set; }

        public string offExpireDate { get; set; }
        public short offtype { get; set; }
        public int Favorates { get; set; }
        public int LetmeKnows { get; set; }

        public BrandShortVM Brand { get; set; }
        public productStateShortVM ProductState { get; set; }

        public prcatShortVM ProductCategory { get; set; }
        public IEnumerable<productpriceShortVM> ProductPrices { get; set; }
        public IEnumerable<productcolorShortVM> Colors { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public IEnumerable<string> Models { get; set; }
        public IEnumerable<productgarantyShortVM> Garanties { get; set; }
        public IEnumerable<PAttributeSelect> PAttributeSelect { get; set; }
        public PrGallery PrGallery { get; set; }
        public IEnumerable<tagvm> Tags{ get; set; }
        public string Data { get; set; }
        public IEnumerable<ProductAttributeGroupSelectsvm> groups { get; set; }
        public IEnumerable<string> ProductAdvantage { get; set; }
        public IEnumerable<string> ProductCommentDisAdvantage { get; set; }
        public IEnumerable<keynameIdvm> ProductRanks{ get; set; }
        public IEnumerable<rankvm> ProductRankSelectValues { get; set; }

        #endregion
    }
    public class ProductAttributeGroupSelectsvm
    {
        public IEnumerable<groupAttsvm> ProductAttributeGroupSelects { get; set; }
    }
    public class groupAttsvm
    {
        public string grouptitle { get; set; }
        public string attname { get; set; }
        public int attid { get; set; }
        public int DisplayGroupOrder { get; set; }
        public int DisplayOrder { get; set; }
        public string Unit { get; set; }
        public IEnumerable<attItemvm> items { get; set; }
        public int DataType { get; set; }
        public string Descr { get; set; }
    }
    public class attItemvm
    {
        public int Id { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; }
    }
    public class keynameIdvm
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class rankvm
    {
        public int RankId { get; set; }
        public double value { get; set; }
    }
    public class tagvm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TagName { get; set; }
    }
    public class PrGallery
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int ProductId { get; set; }
        public string MainImageFileName { get; set; }
        public IEnumerable<productImageShortVM> OtherImages { get; set; }
        public IEnumerable<productImageShortVM> ProductOtherImages { get; set; }
        public IEnumerable<productImageShortVM> UserOtherImages { get; set; }
        public Guid? VideoId { get; set; }
        public string Video { get; set; }
        public string VideoCover { get; set; }
        public string VideoTitle { get; set; }
        public string VideoOnbox { get; set; }
        public Guid? VideoOnboxId { get; set; }
        public string VideoOnboxTitle { get; set; }
        public string VideoOnboxCover { get; set; }
        public Guid? VideoInstallationGuideId { get; set; }
        public string VideoInstallationGuide { get; set; }
        public string VideoInstallationGuideTitle { get; set; }
        public string VideoInstallationGuideCover { get; set; }
        public Guid? AudioId { get; set; }
        public string Audio { get; set; }
        public string AudioCover { get; set; }
    }
    public class BrandShortVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string persianname { get; set; }
        public string image { get; set; }
    }
    public class prcatShortVM
    {
        public int id { get; set; }
        public string name { get; set; }
        public string pageaddress { get; set; }
    } 
    public class productStateShortVM
    {
        public int id { get; set; }
        public string name { get; set; }
    } 
    public class productImageShortVM
    {
        public string prname { get; set; }
        public string prtitle { get; set; }
        public string filename { get; set; }
    }
    public class productcolorShortVM
    {
        public int id { get; set; }
        public string value { get; set; }
        public string color { get; set; }
    }
    public class productgarantyShortVM
    {
        public int id { get; set; }
        public string value { get; set; }
        public string Descr { get; set; }
    }
    public class productpriceShortVM
    {
        public int id { get; set; }
        public string ProductAttributeSelectColorId { get; set; }
        public string ProductAttributeSelectModelId { get; set; }
        public string ProductAttributeSelectSizeId { get; set; }
        public string ProductAttributeSelectWeightId { get; set; }
        public string ProductAttributeSelectGarantyId { get; set; }
    }
    public class orderMergeVM
    {
        public string Id { get; set; }
        public string CustomerOrderId { get; set; }
        public string InsertDate { get; set; }
        public long Price { get; set; }
        public string State { get; set; }
        public int OrderDeliveryId { get; set; }
        public IEnumerable<OrderRowsVM> OrderRows { get; set; }
    }
    public class OrderDeliveryVM
    {
        public int Id { get; set; }
    }
    public class OrderRowsVM
    {
        public int Id { get; set; }
        public string OrderId { get; set; }
        public string MainImage { get; set; }
        public string Title { get; set; }
    }
}
