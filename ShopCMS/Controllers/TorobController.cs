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
    public class TorobController : ApiController
    {
        UnitOfWorkClass uow = null;
        public TorobController()
        {
            uow = new UnitOfWorkClass();
        }
        // GET: api/Torob/5
        [HttpPost]

        public async Task<JsonResult<Domain.ViewModels.TotobItem>> products()
        {
            try
            {
                int? page_unique = null;
                int? page = null;
                string page_url = null;


                var result = Request.Content.ReadAsStringAsync().Result;
                if (result.ToLower().Contains("name="))
                {
                    var list = result.Split(new string[] { "name=" }, StringSplitOptions.None);
                    if (list.Any())
                    {
                        for (int i = 0; i < list.Length; i++)
                        {
                            list[i] = list[i].Replace("\r", "").Replace("\n", "").Replace("\"", "");
                        }
                        if (list.Where(x => x.StartsWith("page_unique")).Any())
                            //page_unique = getnumber(list.Where(x => x.StartsWith("page_unique")).First().Split(new string[] { "------" }, StringSplitOptions.None).First());
                            page_unique = Convert.ToInt32(list.Where(x => x.StartsWith("page_unique")).First().Split(new string[] { "--" }, StringSplitOptions.None).First().ToLower().Replace("page_unique", ""));
                        if (list.Where(x => x.StartsWith("page") && !x.StartsWith("page_unique") && !x.StartsWith("page_url")).Any())
                            //page = getnumber(list.Where(x => x.StartsWith("page") && !x.StartsWith("page_unique") && !x.StartsWith("page_url")).First().Split(new string[] { "------" }, StringSplitOptions.None).First());
                            page = Convert.ToInt32(list.Where(x => x.StartsWith("page") && !x.StartsWith("page_unique") && !x.StartsWith("page_url")).First().Split(new string[] { "--" }, StringSplitOptions.None).First().ToLower().Replace("page", ""));
                        if (list.Where(x => x.StartsWith("page_url")).Any())
                            //page_url = list.Where(x => x.StartsWith("page_url")).First().Split(new string[] { "------" }, StringSplitOptions.None).First().Replace("page_url", "");
                            page_url = list.Where(x => x.StartsWith("page_url")).First().Split(new string[] { "--" }, StringSplitOptions.None).First().Replace("page_url", "");

                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(result))
                    {
                        if (result.ToLower().Contains("page"))
                        {
                            var m = JsonConvert.DeserializeObject<TorobSend>(result);
                            page_unique = m.page_unique;
                            page_url = m.page_url;
                            page = m.page;
                        }
                    }

                }

                if (!page_unique.HasValue && String.IsNullOrEmpty(page_url))
                {
                    int pg = 1;
                    if (page.HasValue)
                        pg = page.Value;
                    int skip = (pg - 1) * 100;
                    return Json(uow.ProductRepository.TorobListPagging(x => x.IsActive && x.ProductPrices.Any(p => p.IsDefault) && x.state == 4, x => x.OrderByDescending(o => o.Id), skip, 100));
                }
                else
                {
                    int id = 0;
                    if (page_unique.HasValue)
                        id = page_unique.Value;
                    else if (page_url != null)
                        //id = int.Parse(page_url.Substring(page_url.ToLower().LastIndexOf("tfp/") + 4, page_url.Length - page_url.ToLower().LastIndexOf("tfp/") - 4));
                        id = int.Parse(page_url.Split('/')[4]);
                    return Json(uow.ProductRepository.TorobList(x => x.Id == id && x.IsActive && x.ProductPrices.Any(p => p.IsDefault) && x.state == 4, x => x.OrderByDescending(o => o.Id)));

                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public class TorobSend
        {
            public int? page_unique { get; set; }
            public string page_url { get; set; }
            public int? page { get; set; }
        }
        public async Task<JsonResult<TorobPr>> get222()
        {

            return Json(new TorobPr());
        }

        // POST: api/Torob
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Torob/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/Torob/5
        public void Delete(int id)
        {
        }
        public int getnumber(string str)
        {
            string a = str;
            string b = string.Empty;
            int val = 0;

            for (int i = 0; i < a.Length; i++)
            {
                if (Char.IsDigit(a[i]))
                    b += a[i];
            }

            if (b.Length > 0)
                val = int.Parse(b);

            return val;
        }
    }
    public class TorobPr
    {
        public string page_unique { get; set; }

    }
}
