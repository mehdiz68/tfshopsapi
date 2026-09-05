using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TfShop.ViewModels.Api.HeaderVM
{
    public class PrCatVM
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string pageAdress { get; set; }
        public int sort { get; set; }
        public bool hidden { get; set; }
        public string cover { get; set; }
        public string mobileIcon { get; set; }
        public string mobileCover { get; set; }
        public List<PrCatVMChild> Child { get; set; }
    }
    public class PrCatVMChild
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string pageAdress { get; set; }
        public int sort { get; set; }
        public bool hidden { get; set; }
        public string cover { get; set; }
        public string mobileIcon { get; set; }
        public string mobileCover { get; set; }
        public List<PrCatVMSubChild> Child { get; set; }
    }
    public class PrCatVMSubChild
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string title { get; set; }
        public string pageAdress { get; set; }
        public int sort { get; set; }
        public bool hidden { get; set; }
        public string cover { get; set; }
        public string mobileIcon { get; set; }
        public string mobileCover { get; set; }
        public List<PrCatVM> Child { get; set; }
    }
    public class socialVM
    {
        public int Id { get; set; }
        public string cover { get; set; }
        public string title { get; set; }
        public string socialLink { get; set; }
        public string icon { get; set; }

    }
}