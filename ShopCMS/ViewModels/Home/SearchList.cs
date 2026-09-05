using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TfShop.ViewModels.Home
{
    public class SearchList
    {
        public SearchList(int id,string title,string oabstract,int typid,Guid? img,string urlContent)
        {
            UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass(); 
            this.Id = id;
            this.Title = title;
            this.Abstract = oabstract;
            this.TypeId = typid;
           
                if (img.HasValue)
                    this.Img = urlContent + uow.AttachmentRepository.GetByID(img).FileName;
                else
                    this.Img = "/Content/Default/images/default-thumbnail.jpg";


            if (typid == -5)
                PagaAdress = "AdCategory/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == -4)
                PagaAdress = "Ads/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == -3)
                PagaAdress = "TFC/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == -2)
                PagaAdress = "TFP/"+id+"/"+ CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == -1)
                PagaAdress = "tag/" + id+"/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid == 0)
                PagaAdress = "category/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
            else if (typid > 0)
                PagaAdress = "content/" + id + "/" + CoreLib.Infrastructure.CommonFunctions.NormalizeAddress(title);
        }
        #region Properties

        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string PagaAdress { get; set; }
        public int TypeId { get; set; }
        public string Img { get; set; }


        #endregion

    }

    public class MainSearch
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Title { get; set; }
        public string PageAddress { get; set; }
        public int TypeId { get; set; }
        public string AttachementId { get; set; }
    }


    public class PrsShowMain
    {
        public string Title { get; set; }
        public int CatId { get; set; }
        public int c { get; set; }
        public decimal raito { get; set; }
        public decimal finalraito { get; set; }
    }
    public class PrsReferer
    {
        public string referer { get; set; }
        public DateTime LogDateTime { get; set; }
    }
    public class PrsUserCount
    {
        public int c { get; set; }
    }
    public class PrsRefererCount
    {
        public int Id { get; set; }
        public int c { get; set; }
        public int? parrentId { get; set; }
    }
    public class PrsRefererCountTitle
    {
        public string Link { get; set; }
        public string title { get; set; }
        public string cover { get; set; }
        public int c { get; set; }
        public double percent { get; set; }
    }
    public class PrsRefererROI
    {
        public string Link { get; set; }
        public string title { get; set; }
        public string cover { get; set; }
        public int c { get; set; }
        public double percent { get; set; }
        public int orderCount { get; set; }
        public double ROI { get; set; }
        public long Cost { get; set; }
        public long OrderSum { get; set; }
        public long grossProfit { get; set; }
        public long NetProfit { get; set; }
    }


    public class PrsView
    {
        public IEnumerable<PrsShowMain> PrsShowMains { get; set; }
        public IEnumerable<PrsUserCount> PrsUserCounts { get; set; }
        public string FirstVisit { get; set; }
        public string LastVisit { get; set; }
    }

    public class UserActivityLive
    {
        public int? CookieId { get; set; }
        public string LOGON_USER { get; set; }
        public string IP { get; set; }
        public string Platform { get; set; }
        public string Browser { get; set; }
        public DateTime LogDateTime { get; set; }
        public string URL { get; set; }
        public string Referer { get; set; }
        public int countVisit { get; set; }
    }
}