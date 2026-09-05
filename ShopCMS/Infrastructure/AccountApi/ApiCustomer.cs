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
    public static class ApiCustomer
    {

        public static async Task<ApiCustomerReturnResult> AddCustomer(string token, ApplicationUser user)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {


                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/Customer");
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
                        HttpResponseMessage response = await client.GetAsync("http://91.98.128.188:8080/TncHoloo/api/Customer?Mobile=" + user.PhoneNumber);
                        var x = response.Content.ReadAsStringAsync().Result;
                        if (x== "{}")
                        {

                            String jsonString = JsonConvert.SerializeObject(new ApiCustomerResult
                            {
                                custinfo = new custinfo
                                {
                                    name = string.Format("{0} {1}", user.FirstName, user.LastName),
                                    custtype = 0,
                                    ispurchaser = true,
                                    isseller = true,
                                    nationalid = user.NationalCode,
                                    mobile = user.PhoneNumber,
                                    city = user.CityId.HasValue ? user.CityEntity.Name : "",
                                    ostan = user.CityId.HasValue ? user.CityEntity.Province.Name : "",
                                    address = string.Format("{0}{1}{2}", user.Address, (!String.IsNullOrEmpty(user.AddressNumber) ? " ، پلاک" + user.AddressNumber : ""), (!String.IsNullOrEmpty(user.AddressUnit) ? " ، واحد" + user.AddressUnit : "")),
                                    email = user.Email,
                                    tel = user.LandlinePhone,
                                    zipcode = user.PostalCode
                                }
                            });

                            req = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
                            req.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");


                            response = await client.SendAsync(req);

                            if (response.IsSuccessStatusCode)
                            {
                                var result = response.Content.ReadAsStringAsync().Result;
                                var r = JsonConvert.DeserializeObject<ApiCustomerReturnResult>(result);
                                if (r.Success != null)
                                {
                                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "ثبت مشتری جدید در حسابداری", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return r;
                                }

                                else
                                {
                                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "خطا در ثبت مشتری در حسابداری" + r.Failure.Error + "-erroCode:" + r.Failure.ErrorCode, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return null;
                                }
                            }

                            EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "خطا در ثبت مشتری جدید در حسابداری" + response.Content.ReadAsStringAsync().Result, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return null;
                        }
                        else
                        {
                            EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "مشتری در حسابداری وجود دارد", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 500, "خطا در ورود به API حسابداری" + ex.Message, DateTime.Now, "b11c3d7b -4d2d-4b58-85f1-2e8e0c8d6d84");
                    return null;
                }
            }
            else
            {
                EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 500, "عدم احراز هویت برای ثبت مشتری جدید", DateTime.Now, "b11c3d7b -4d2d-4b58-85f1-2e8e0c8d6d84");
                return null;
            }
        }


        public static async Task<ApiCustomerReturnResult> UpdateCustomer(string token, ApplicationUser user)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {


                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/Customer");
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
                        HttpResponseMessage response = await client.GetAsync("http://91.98.128.188:8080/TncHoloo/api/Customer?Mobile=" + user.PhoneNumber);
                        var x = response.Content.ReadAsStringAsync().Result;
                        if (x == "{}") //add
                        {
                            client.DefaultRequestHeaders.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            client.DefaultRequestHeaders.Add("Authorization", token);

                            String jsonString = JsonConvert.SerializeObject(new ApiCustomerResult
                            {
                                custinfo = new custinfo
                                {
                                    name = string.Format("{0} {1}", user.FirstName, user.LastName),
                                    custtype = 0,
                                    ispurchaser = true,
                                    isseller = true,
                                    nationalid = user.NationalCode,
                                    mobile = user.PhoneNumber,
                                    city = user.CityId.HasValue ? user.CityEntity.Name : "",
                                    ostan = user.CityId.HasValue ? user.CityEntity.Province.Name : "",
                                    address = string.Format("{0}{1}{2}", user.Address, (!String.IsNullOrEmpty(user.AddressNumber) ? " ، پلاک" + user.AddressNumber : ""), (!String.IsNullOrEmpty(user.AddressUnit) ? " ، واحد" + user.AddressUnit : "")),
                                    email = user.Email,
                                    tel = user.LandlinePhone,
                                    zipcode = user.PostalCode
                                }
                            });

                            req = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
                            req.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");


                            response = await client.SendAsync(req);

                            if (response.IsSuccessStatusCode)
                            {
                                var result = response.Content.ReadAsStringAsync().Result;
                                var r = JsonConvert.DeserializeObject<ApiCustomerReturnResult>(result);
                                if (r.Success != null)
                                {
                                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "ثبت مشتری جدید در حسابداری", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return r;
                                }
                                else
                                {
                                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "خطا در ثبت مشتری در حسابداری" + r.Failure.Error + "-erroCode:" + r.Failure.ErrorCode, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return null;
                                }
                            }

                            EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "خطا در ثبت مشتری جدید در حسابداری" + response.Content.ReadAsStringAsync().Result, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return null;
                        }
                        else //update
                        {
                            var cutinfo = JsonConvert.DeserializeObject<ApiCustomerInfo>(x);
                            var jsonString = new ApiCustomerResult
                            {
                                custinfo = new custinfo
                                {
                                    ErpCode = cutinfo.Customer.First().ErpCode,
                                    name = string.Format("{0} {1}", user.FirstName, user.LastName),
                                    nationalid = user.NationalCode,
                                    city = user.CityId.HasValue ? user.CityEntity.Name : "",
                                    ostan = user.CityId.HasValue ? user.CityEntity.Province.Name : "",
                                    address = string.Format("{0}{1}{2}", user.Address, (!String.IsNullOrEmpty(user.AddressNumber) ? " ، پلاک" + user.AddressNumber : ""), (!String.IsNullOrEmpty(user.AddressUnit) ? " ، واحد" + user.AddressUnit : "")),
                                    email = user.Email,
                                    tel = user.LandlinePhone,
                                    zipcode = user.PostalCode,
                                    ispurchaser = true,
                                    isseller = true,
                                    mobile = user.PhoneNumber
                                }
                            };
                            response = await client.PutAsJsonAsync(client.BaseAddress, jsonString);

                            if (response.IsSuccessStatusCode)
                            {
                                var result = response.Content.ReadAsStringAsync().Result;
                                var r = JsonConvert.DeserializeObject<ApiCustomerReturnResult>(result);
                                if (r.Success != null)
                                {
                                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "ویرایش مشتری در حسابداری", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return r;
                                }
                                else
                                {
                                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "خطا در ویرایش مشتری در حسابداری" + r.Failure.Error + "-erroCode:" + r.Failure.ErrorCode, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return null;
                                }
                            }
                            EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "خطا در ویرایش مشتری در حسابداری" + response.Content.ReadAsStringAsync().Result, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 500, "خطا در ورود به API حسابداری" + ex.Message, DateTime.Now, "b11c3d7b -4d2d-4b58-85f1-2e8e0c8d6d84");
                    return null;
                }
            }
            else
            {
                EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 500, "عدم احراز هویت برای ثبت مشتری جدید", DateTime.Now, "b11c3d7b -4d2d-4b58-85f1-2e8e0c8d6d84");
                return null;
            }
        }



        public static async Task<ApiCustomerInfo> GetAllCutomer(string token)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {


                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/Customer");
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, client.BaseAddress);
                        HttpResponseMessage response = await client.GetAsync("http://91.98.128.188:8080/TncHoloo/api/Customer");
                        var x = response.Content.ReadAsStringAsync().Result;
                        var Allcutinfo = JsonConvert.DeserializeObject<ApiCustomerInfo>(x);
                        return Allcutinfo;
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

        public static async Task<ApiCustomerReturnResult> UPCustomer(string token, ApplicationUser user, ApiCustomerInfo Allcutinfo)
        {
            if (!String.IsNullOrEmpty(token))
            {
                try
                {


                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/Customer");
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Add("Authorization", token);
                        HttpResponseMessage response = null;
                        if (Allcutinfo.Customer.Any(s => s.NationalId.PersianToEnglish() == user.NationalCode)) //update
                        {
                            var cutinfo = Allcutinfo.Customer.Where(s => s.NationalId.PersianToEnglish() == user.NationalCode).First();
                            var jsonString = new ApiCustomerResult
                            {
                                custinfo = new custinfo
                                {
                                    ErpCode = cutinfo.ErpCode,
                                    name = string.Format("{0} {1}", user.FirstName, user.LastName),
                                    nationalid = user.NationalCode,
                                    city = user.CityId.HasValue ? user.CityEntity.Name : "",
                                    ostan = user.CityId.HasValue ? user.CityEntity.Province.Name : "",
                                    address = string.Format("{0}{1}{2}", user.Address, (!String.IsNullOrEmpty(user.AddressNumber) ? " ، پلاک" + user.AddressNumber : ""), (!String.IsNullOrEmpty(user.AddressUnit) ? " ، واحد" + user.AddressUnit : "")),
                                    email = user.Email,
                                    tel = user.LandlinePhone,
                                    zipcode = user.PostalCode,
                                    ispurchaser = true,
                                    isseller = true,
                                    mobile = user.PhoneNumber
                                }
                            };
                            response = await client.PutAsJsonAsync(client.BaseAddress, jsonString);

                            if (response.IsSuccessStatusCode)
                            {
                                var result = response.Content.ReadAsStringAsync().Result;
                                var r = JsonConvert.DeserializeObject<ApiCustomerReturnResult>(result);
                                if (r.Success != null)
                                {
                                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "ویرایش مشتری در حسابداری", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return r;
                                }
                                else
                                {
                                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "خطا در ویرایش مشتری در حسابداری" + r.Failure.Error + "-erroCode:" + r.Failure.ErrorCode, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                                    return null;
                                }
                            }
                        }
                        EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 200, "خطا در ویرایش مشتری در حسابداری" + (response != null ? response.Content.ReadAsStringAsync().Result : ""), DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 500, "خطا در ورود به API حسابداری" + ex.Message, DateTime.Now, "b11c3d7b -4d2d-4b58-85f1-2e8e0c8d6d84");
                    return null;
                }
            }
            else
            {
                EventLog.Logger.Add(5, "ApiCustomer", "AddCustomer", true, 500, "عدم احراز هویت برای ثبت مشتری جدید", DateTime.Now, "b11c3d7b -4d2d-4b58-85f1-2e8e0c8d6d84");
                return null;
            }
        }
    }
    public class ApiCustomerResult
    {
        public custinfo custinfo { get; set; }
    }
    public class custinfo
    {

        public string ErpCode { get; set; }
        public string name { get; set; }
        public bool ispurchaser { get; set; }
        public bool isseller { get; set; }
        public int custtype { get; set; }
        public string nationalid { get; set; }
        public string tel { get; set; }
        public string mobile { get; set; }
        public string city { get; set; }
        public string ostan { get; set; }
        public string email { get; set; }
        public string zipcode { get; set; }
        public string address { get; set; }
    }

    public class ApiCustomerReturnResult
    {
        public string Header { get; set; }
        public CustomerSuccess Success { get; set; }
        public CustomerFailure Failure { get; set; }
    }
    public class CustomerSuccess
    {

        public string Id { get; set; }
        public string ErpCode { get; set; }
        public string ReturnParam1 { get; set; }
        public string ReturnParam2 { get; set; }
        public string ReturnParam3 { get; set; }
    }
    public class CustomerFailure
    {
        public string Error { get; set; }
        public string ErrorCode { get; set; }
    }

    public class ApiCustomerInfo
    {
        public List<CustomerInfo> Customer { get; set; }
    }

    public class CustomerInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NationalId { get; set; }
        public string Mobile { get; set; }
        public string ErpCode { get; set; }

    }
}