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

namespace TfShop.Infrastructure.AccountApi
{
    public static class ApiLogin
    {

        public static async Task<LoginResult> DoLogin()
        {
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://91.98.128.188:8080/TncHoloo/api/Login");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    String jsonString = JsonConvert.SerializeObject(new loginInfo { userinfo = new userinfo { dbname = "holoo1", username = "web", userpass = Base64Encode("1") } });

                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
                    req.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");


                    HttpResponseMessage response = await client.SendAsync(req);

                    if (response.IsSuccessStatusCode)
                    {
                        string res = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<LoginResult>(res);
                        if (result.Login.State == true)
                        {
                            TfShop.Infrastructure.EventLog.Logger.Add(5, "ApiLogin", "DoLogin", true, 200, "ورود به API حسابداری", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return result;
                        }
                        else
                        {

                            TfShop.Infrastructure.EventLog.Logger.Add(5, "ApiLogin", "DoLogin", true, 500, "خطا در ورود به API حسابداری" + res, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            return null;
                        }
                    }

                    TfShop.Infrastructure.EventLog.Logger.Add(5, "ApiLogin", "DoLogin", true, 500, "خطا در ورود به API حسابداری", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                    return null;
                }
            }
            catch (Exception ex)
            {
                TfShop.Infrastructure.EventLog.Logger.Add(5, "ApiLogin", "DoLogin", true, 500, "خطا در ورود به API حسابداری" + ex.Message, DateTime.Now, "b11c3d7b -4d2d-4b58-85f1-2e8e0c8d6d84");
                return null;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
    public class LoginResult
    {
        public Login Login { get; set; }
    }
    public class Login
    {

        public bool State { get; set; }
        public string Token { get; set; }
    }
    public class loginInfo
    {
        public userinfo userinfo { get; set; }
    }
    public class userinfo
    {

        public string username { get; set; }
        public string userpass { get; set; }
        public string dbname { get; set; }
    }
}