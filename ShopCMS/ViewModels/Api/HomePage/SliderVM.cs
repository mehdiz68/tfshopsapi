using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TfShop.ViewModels.Api.HomePage
{
    public class TitleLink
    {
        public string title { get; set; }
        public string link { get; set; }
    }
    public class singleComment
    {
        public int Id { get; set; }
        public int? ParrentId { get; set; }
        public string FullName { get; set; }
        public string InsertDate { get; set; }
        public string InsertTime { get; set; }
        public int PositiveRating { get; set; }
        public int NegativeRating { get; set; }
        public string Message { get; set; }
        public List<singleComment> ChildComments { get; set; }
    }
    public class Master
    {
        public string image { get; set; }
        public string link { get; set; }
        public string title { get; set; }
        public string pageaddress { get; set; }
        public string name { get; set; }
        public DateTime insertdate { get; set; }
    }
    public class Master2
    {
        public int id { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public string pageaddress { get; set; }
        public string insertdate { get; set; }
    }
    public class Master3
    {
        public int id { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public string pageaddress { get; set; }
        public DateTime insertdate { get; set; }
        public int visits { get; set; }
        public int readMinuts { get; set; }
        public string abst { get; set; }
        public int? catid { get; set; }
        public string cattitle { get; set; }
        public string catpageaddress { get; set; }
    }
    public class Master4
    {
        public int id { get; set; }
        public string image { get; set; }
        public string title { get; set; }
        public string pageaddress { get; set; }
        public DateTime insertdate { get; set; }
        public string duration { get; set; }
    }
    public class Master5
    {
        public int id { get; set; }
        public string image { get; set; }
        public string videosrc { get; set; }
        public string audiosrc { get; set; }
        public string title { get; set; }
        public string pageaddress { get; set; }
        public DateTime insertdate { get; set; }
        public bool audio { get; set; }
        public string abst { get; set; }
        public int visits { get; set; }
    }
    public class Master6
    {
        public int id { get; set; }
        public string image { get; set; }
        public string video { get; set; }
        public string audio { get; set; }
        public string title { get; set; }
        public string pageaddress { get; set; }
        public string insertdate { get; set; }
        public string inserttime { get; set; }
        public int visits { get; set; }
        public int readMinuts { get; set; }
        public string abst { get; set; }
        public int? catid { get; set; }
        public string cattitle { get; set; }
        public string catpageaddress { get; set; }
        public int duration { get; set; }
        public string contenttype { get; set; }
        public int likeCount { get; set; }
    }
    public class singleContent : Master6
    {
        public int contenttypeid { get; set; }
        public double rate { get; set; }
        public double rateCount { get; set; }
        public string data { get; set; }
        public bool hasContent { get; set; }
        public List<TitleLink> sources { get; set; }
        public List<TitleLink> tags { get; set; }
        public List<TitleLink> fags { get; set; }
        public List<TitleLink> images { get; set; }
        public List<singleComment> comments { get; set; }
    }
    public class breadcrumbCat
    {
        public int contenttypeid { get; set; }
        public string catpageaddress { get; set; }
        public string cattitle { get; set; }
        public int? catid { get; set; }
    }
    public class RelatedContent:Master2
    {
        public int ContentTypeId { get; set; }
    }
}