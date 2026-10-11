using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TfShop.Infrastructure.Newsletter;

namespace TfShop.Controllers
{
    public class HomeController : Controller
    {
        private UnitOfWork.UnitOfWorkClass uow = null;

        public HomeController()
        {
            uow = new UnitOfWork.UnitOfWorkClass();
        }

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// تاییدِ ایمیلِ خبرنامه - مقصدِ لینکی که apiv1/JoinNewsLetter ایمیل می‌کند.
        ///
        /// کاربر این لینک را از داخلِ ایمیلش باز می‌کند نه از داخلِ سایت، پس این‌جا صفحه
        /// برمی‌گردد نه JSON. Viewهای Home موقعِ استخراجِ API حذف شده‌اند، پس HTML مستقیم
        /// برگردانده می‌شود و کاربر با یک دکمه به سایتِ اصلی برمی‌گردد.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult VerifyEmail(string email, string token)
        {
            string site = System.Configuration.ConfigurationManager.AppSettings["NextJsFrontendUrl"] ?? "https://www.tfshops.com";

            try
            {
                string address = (email ?? "").Trim();

                if (string.IsNullOrEmpty(address) || !NewsLetterToken.IsValid(address, token))
                    return VerifyPage(false, "لینک تایید معتبر نیست یا ناقص باز شده است.", site);

                var record = uow.NewsLetterEmailRepository
                    .Get(x => x, x => x.Email == address).FirstOrDefault();

                if (record == null)
                    return VerifyPage(false, "این ایمیل در خبرنامه ثبت نشده است.", site);

                if (record.IsVerified)
                    return VerifyPage(true, "ایمیل شما قبلاً تایید شده بود.", site);

                record.IsVerified = true;
                uow.NewsLetterEmailRepository.Update(record);
                uow.Save();

                return VerifyPage(true, "ایمیل شما تایید شد. از این به بعد خبرها و تخفیف‌ها را برایتان می‌فرستیم.", site);
            }
            catch (Exception ex)
            {
                Infrastructure.EventLog.Logger.Add(5, "home", "verifyemail-newsletter", false, 500, ex.Message,
                    DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

                return VerifyPage(false, "خطایی رخ داد. لطفاً بعداً دوباره تلاش کنید.", site);
            }
        }

        private ContentResult VerifyPage(bool ok, string message, string site)
        {
            string color = ok ? "#2e9e5b" : "#d0342c";
            string title = ok ? "تایید خبرنامه" : "تایید نشد";

            string html =
                "<!DOCTYPE html><html lang=\"fa\" dir=\"rtl\"><head><meta charset=\"utf-8\">"
                + "<meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">"
                + "<title>" + title + "</title></head>"
                + "<body style=\"margin:0;padding:40px 16px;background:#fafafa;"
                + "font-family:Tahoma,Arial,sans-serif;color:#333;text-align:center\">"
                + "<div style=\"max-width:420px;margin:0 auto;background:#fff;border:1px solid #ededed;"
                + "border-radius:14px;padding:28px 20px\">"
                + "<h1 style=\"margin:0 0 14px;font-size:17px;color:" + color + "\">" + title + "</h1>"
                + "<p style=\"margin:0 0 22px;font-size:13px;line-height:2.1\">" + Server.HtmlEncode(message) + "</p>"
                + "<a href=\"" + Server.HtmlEncode(site) + "\" style=\"display:inline-block;padding:11px 22px;"
                + "background:#f4633a;color:#fff;border-radius:10px;font-size:13px;text-decoration:none\">"
                + "بازگشت به تی اف شاپ</a>"
                + "</div></body></html>";

            return Content(html, "text/html", System.Text.Encoding.UTF8);
        }
    }
}
