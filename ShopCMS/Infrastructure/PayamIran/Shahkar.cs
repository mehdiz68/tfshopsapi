using System;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;

namespace TfShop.Infrastructure.PayamIran
{
    public static class Shahkar
    {
        public static async Task<Token> GetTokenCode()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://op1.pgsb.ir/oauth/token");
                    request.Headers.Add("ContentType", "x-www-form-urlencoded");
                    request.Headers.Add("Authorization", "Basic MjlhNjk2ZTc5ODQ4NGFlNDg0MDlmYTlhZjdkOWViY2U6MnpOMFcwQVdTMTJVNTRKMw==");
                    var collection = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("username", "tehranlida"),
                    new KeyValuePair<string, string>("password", "zZ1kaH5n!f"),
                    new KeyValuePair<string, string>("grant_type", "password")
                };

                    var content = new FormUrlEncodedContent(collection);
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();


                    Token res = System.Text.Json.JsonSerializer.Deserialize<Token>(await response.Content.ReadAsStringAsync());

                    if (res != null)
                        return res;
                    else
                    {
                        EventLog.Logger.Add(1, "Shahkar", "CheckCode", true, 200, " خطا در لاگین شاهکار، کد خطا " + await response.Content.ReadAsStringAsync(), DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.Logger.Add(1, "Shahkar", "CheckCode", true, 200, " خطا در لاگین شاهکار، کد خطا : " + ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                return null;
            }

        }
        public static async Task<string> CheckCode(string token, string mobile, string code)
        {
            string shahkarCompanyCode = "0101";

            string tokenIdentificationNo = "", tokenServiceNumber = "";
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                // nationalCode 
                var vm = new { val = code };
                var dataString = JsonConvert.SerializeObject(vm);
                string data = client.UploadString("http://ShkAPI.tfshops.com/Home", "POST", dataString);
                tokenIdentificationNo = data.ToString();
                EventLog.Logger.Add(1, "Shahkar", "CheckCode", true, 200, " کدگذاری کد ملی " + tokenIdentificationNo, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
            }
            using (var client = new WebClient())
            {
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                // mobileNumber 
                var vm2 = new { val = mobile };
                string dataString2 = JsonConvert.SerializeObject(vm2);
                string data2 = client.UploadString("http://ShkAPI.tfshops.com/Home", "POST", dataString2);
                tokenServiceNumber = data2.ToString();
                EventLog.Logger.Add(1, "Shahkar", "CheckCode", true, 200, " کدگذاری موبایل" + tokenServiceNumber, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

            }
            using (var client2 = new HttpClient())
            {
                var request2 = new HttpRequestMessage(HttpMethod.Post, "https://op1.pgsb.ir/api/client/apim/v1/shahkaar/gwsh/ser-viceIDmatchingencrypted");
                request2.Headers.Add("pid", "65dc8c920ad7207bb5c0f9f1");
                //request2.Headers.Add("Content-Type", "application/json");
                request2.Headers.Add("basicAuthorization", "Basic MjlhNjk2ZTc5ODQ4NGFlNDg0MDlmYTlhZjdkOWViY2U6MnpOMFcwQVdTMTJVNTRKMw==");
                request2.Headers.Add("authorizationCode", "Bearer " + token);


                string timeString = DateTime.Now.ToString("HHmmssfff");
                string requestId = shahkarCompanyCode + DateTime.Now.ToString("yyyyMMdd") + timeString + "000";
                string body = "{" +
                              "\n\"requestId\":\"" + requestId + "\"," +
                              "\n\"serviceNumber\": \"" + tokenServiceNumber + "\"," +
                              "\n\"identificationNo\": \"" + tokenIdentificationNo + "\"," +
                              "\n\"identificationType\": 0," +
                              "\n\"serviceType\": 2" +
                              "\n}";
                var content2 = new StringContent(body, Encoding.UTF8, "application/json");
                Console.WriteLine("body ==>> " + body);
                request2.Content = content2;
                EventLog.Logger.Add(1, "Shahkar", "CheckCode", true, 200, " ارسال درخواست" + body, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

                var response2 = await client2.SendAsync(request2);
                response2.EnsureSuccessStatusCode();
                EventLog.Logger.Add(1, "Shahkar", "CheckCode", true, 200, await response2.Content.ReadAsStringAsync(), DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

                return await response2.Content.ReadAsStringAsync();
            }

        }

        //public static async bool CheckCode(string token, string mobile, string code, out string log)
        //{
        //    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });

        //    try
        //    {
        //        using (var client = new WebClient())
        //        {
        //            client.Headers.Clear();
        //            client.Headers.Add("Authorization", "Bearer " + token);
        //            var vm = new { mobileNumber = mobile, nationalCode = code };
        //            var dataString = JsonConvert.SerializeObject(vm);
        //            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

        //            string json = client.UploadString("https://gsafe.ir:52000/rose/gss/api/Shahkar", "POST", dataString);
        //            dynamic data = JsonConvert.DeserializeObject(json);

        //            if (Convert.ToBoolean(data.isSuccess) == true)
        //            {
        //                log = data.ToString();
        //                EventLog.Logger.Add(1, "Shahkar", "CheckCode", true, 200, " بررسی صحیح کد ملی " + code + " موبایل " + mobile, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
        //                return true;
        //            }
        //            else
        //            {
        //                if (Convert.ToInt32(data.statusCode) == 401)
        //                {
        //                    log = data.ToString();
        //                    string tkn = await GetTokenCode();
        //                    return CheckCode(tkn, mobile, code, out log);
        //                }
        //                else
        //                {
        //                    log = data.ToString();
        //                    EventLog.Logger.Add(5, "Shahkar", "CheckCode", true, 500, " خطا در بررسی کد ملی " + code + " ، کد خطا : " + data.statusCode, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
        //                    return false;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log = ex.Message;
        //        EventLog.Logger.Add(5, "Shahkar", "CheckCode", true, 500, ex.Message, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
        //        return false;
        //    }
        //}


        //        private static string GetEncryptedToken(string inputData, int inputIat)
        //        {
        //            string publicKeyStringShahkar = @"-----BEGIN PUBLIC KEY-----
        //**********************************************************
        //**********************************************************
        //**********************************************************
        //-----END PUBLIC KEY-----";

        //            // Generates the symmetric key for AES encryption with the algorithm 'A256GCMKW'
        //            var asymmetricJwkKey = AsymmetricJwk.FromPem(publicKeyStringShahkar);
        //            asymmetricJwkKey.KeyOps.Add("");

        //            var payload = new
        //            {
        //                data = inputData,
        //                iat = inputIat
        //            };

        //            string jsonPayload = JsonConvert.SerializeObject(payload);

        //            var asyDescriptorPlainText = new PlaintextJweDescriptor(asymmetricJwkKey, SignatureAlgorithm.HmacSha256 KeyManagementAlgorithm.EcdhEsA256KW, EncryptionAlgorithm.A256Gcm)
        //            {
        //                Payload = jsonPayload
        //            };


        //            // The descriptor sets the 'alg' with value 'A256GCMKW' and 'enc' with value 'A128CBC-HS256'
        //            var writer3 = new JwtWriter();
        //            var token3 = writer3.WriteTokenString(asyDescriptorPlainText);

        //            Console.WriteLine("----------------------------------Start " + inputData +
        //                              " --------------------------------");

        //            Console.WriteLine("The JWT is:");
        //            Console.WriteLine(asyDescriptorPlainText);
        //            Console.WriteLine();
        //            Console.WriteLine("Its compact form is:");
        //            Console.WriteLine(token3);
        //            Console.WriteLine("----------------------------------Finish " + inputData +
        //                              " --------------------------------");

        //            return token3;
        //        }
    }

    public class Token
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public int expires_in { get; set; }
    }
}