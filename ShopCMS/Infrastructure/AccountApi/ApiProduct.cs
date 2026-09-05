using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Domain;
using CoreLib;

namespace TfShop.Infrastructure.AccountApi
{
    public static class ApiProduct
    {

        public static async Task<MainGroupResult> GetAllMainGroup(string token)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/MainGroup");
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
                        HttpResponseMessage response = await client.GetAsync("http://91.98.128.188:8080/TncHoloo/api/MainGroup");
                        var x = response.Content.ReadAsStringAsync().Result;
                        var AllGroups = JsonConvert.DeserializeObject<MainGroupResult>(x);
                        return AllGroups;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static async Task<ProductReturnResult> AddMainGroup(string token, string Name)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {


                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/MainGroup");
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
                        HttpResponseMessage response = await client.GetAsync("http://91.98.128.188:8080/TncHoloo/api/MainGroup");
                        var x = response.Content.ReadAsStringAsync().Result;
                        var Groupinfo = JsonConvert.DeserializeObject<MainGroupResult>(x);
                        if (!Groupinfo.mainGroup.Any(s => s.Name == Name))
                        {
                            MainGroupAddResult mainGroupAddResult = new MainGroupAddResult();
                            mainGroupAddResult.maingroupinfo = new List<MainGroupAddSuccess>();
                            mainGroupAddResult.maingroupinfo.Add(new MainGroupAddSuccess() { name = Name });
                            String jsonString = JsonConvert.SerializeObject(mainGroupAddResult);

                            response = await client.PostAsJsonAsync(client.BaseAddress, jsonString);

                            if (response.IsSuccessStatusCode)
                            {
                                var result = response.Content.ReadAsStringAsync().Result;
                                var r = JsonConvert.DeserializeObject<ProductReturnResult>(result);
                                if (r.Success != null)
                                {
                                    EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 200, "ثبت گروه اصلی در حسابداری", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return r;
                                }
                                else
                                {
                                    EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 200, "خطا در ثبت گروه اصلی در حسابداری" + r.Failure.Error + "-erroCode:" + r.Failure.ErrorCode, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return null;
                                }
                            }
                            EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 200, "خطا در ثبت گروه اصلی در حسابداری" + response.Content.ReadAsStringAsync().Result, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return null;
                        }
                        else
                        {

                            EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 500, "گروه اصلی " + Name + " در حسابداری وجود دارد!", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 500, "خطا در ورود به API حسابداری" + ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                    return null;
                }
            }
            else
            {
                EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 500, "عدم احراز هویت برای ویرایش کالا جدید", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return null;
            }
        }
        public static async Task<SideGroupResult> GetAllSideGroup(string token, string MainGroupErpCode)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/SideGroup");
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
                        HttpResponseMessage response = await client.GetAsync("http://91.98.128.188:8080/TncHoloo/api/SideGroup?MainErpCode=" + MainGroupErpCode);
                        var x = response.Content.ReadAsStringAsync().Result;
                        var AllGroups = JsonConvert.DeserializeObject<SideGroupResult>(x);
                        return AllGroups;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static async Task<ProductResult> GetProduct(string token, string code)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/Product");
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
                        HttpResponseMessage response = await client.GetAsync("http://91.98.128.188:8080/TncHoloo/api/Product?code=" + code);
                        var x = response.Content.ReadAsStringAsync().Result;
                        var Products = JsonConvert.DeserializeObject<ProductResult>(x);
                        return Products;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static async Task<ProductReturnResult> EditProduct(string token, int id, string Code, string ErpCode, string Name, long price)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {


                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/Product");
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
                        HttpResponseMessage response = await client.GetAsync("http://91.98.128.188:8080/TncHoloo/api/Product?ErpCode=" + ErpCode);
                        var x = response.Content.ReadAsStringAsync().Result;
                        if (x != "{}")//update
                        {
                            var productinfo = JsonConvert.DeserializeObject<ProductResult>(x);
                            var jsonString = new ProductInfoResult();
                            jsonString.productinfo = new List<ProductInfo>();
                            jsonString.productinfo.Add(new ProductInfo
                            {
                                id = id,
                                ErpCode = ErpCode,
                                Code = Code,
                                Name = Name,
                                SellPrice = price.ToString()
                            }
                            );
                            response = await client.PutAsJsonAsync(client.BaseAddress, jsonString);

                            if (response.IsSuccessStatusCode)
                            {
                                var result = response.Content.ReadAsStringAsync().Result;
                                var r = JsonConvert.DeserializeObject<ProductReturnResult>(result);
                                if (r.Success != null)
                                {
                                    EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 200, "ویرایش کالا در حسابداری", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return r;
                                }
                                else
                                {
                                    EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 200, "خطا در ویرایش کالا در حسابداری" + r.Failure.Error + "-erroCode:" + r.Failure.ErrorCode, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return null;
                                }
                            }
                            EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 200, "خطا در ویرایش کالا در حسابداری" + response.Content.ReadAsStringAsync().Result, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return null;
                        }
                        else
                        {

                            EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 500, "محصول " + Code + "-" + ErpCode + " در حسابداری پیدا نشد !", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 500, "خطا در ورود به API حسابداری" + ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                    return null;
                }
            }
            else
            {
                EventLog.Logger.Add(5, "ApiProduct", "EditProduct", true, 500, "عدم احراز هویت برای ویرایش کالا جدید", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return null;
            }
        }
    }

    public class MainGroupResult
    {
        public List<MainGroupSuccess> mainGroup { get; set; }
    }
    public class MainGroupSuccess
    {

        public string Name { get; set; }
        public string ErpCode { get; set; }
    }

    public class MainGroupAddResult
    {
        public List<MainGroupAddSuccess> maingroupinfo { get; set; }
    }
    public class MainGroupAddSuccess
    {

        public string name { get; set; }
    }
    public class SideGroupResult
    {
        public List<SideGroupSuccess> sideGroup { get; set; }
    }
    public class SideGroupSuccess
    {

        public string Name { get; set; }
        public string ErpCode { get; set; }
        public string MainGroupName { get; set; }
        public string MainErpCode { get; set; }
    }

    public class ProductResult
    {
        public List<ProductSuccess> product { get; set; }
    }
    public class ProductSuccess
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public string ErpCode { get; set; }
        public string Few { get; set; }
        public string BuyPrice { get; set; }
        public string SellPrice { get; set; }
        public string SellPrice2 { get; set; }
        public string SellPrice3 { get; set; }
        public string SellPrice4 { get; set; }
        public string SellPrice5 { get; set; }
        public string SellPrice6 { get; set; }
        public string SellPrice7 { get; set; }
        public string SellPrice8 { get; set; }
        public string SellPrice9 { get; set; }
        public string SellPrice10 { get; set; }
        public string CountInKaton { get; set; }
        public string CountInBasteh { get; set; }
        public string MainGroupName { get; set; }
        public string MainGroupErpCode { get; set; }
        public string SideGroupName { get; set; }
        public string SideGroupErpCode { get; set; }
    }


    public class ProductInfoResult
    {
        public List<ProductInfo> productinfo { get; set; }
    }
    public class ProductInfo
    {
        public int id { get; set; }
        public string ErpCode { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string SellPrice { get; set; }
    }
    public class ProductReturnResult
    {
        public string Header { get; set; }
        public ProductInfoSuccess Success { get; set; }
        public ProductFailure Failure { get; set; }
    }
    public class ProductInfoSuccess
    {

        public string Id { get; set; }
        public string ErpCode { get; set; }
    }
    public class ProductFailure
    {
        public string Error { get; set; }
        public string ErrorCode { get; set; }
    }
}