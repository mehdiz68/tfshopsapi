using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using UnitOfWork;

namespace TfShop.Infrastructure.EventLog
{
    public static class ClearLog
    {
        public static void Clear()
        {
            DateTime LastMonthDate = DateTime.Now.AddDays(-7);
            DateTime Last3MonthDate = DateTime.Now.AddDays(-90);
            using (DataLayer.TfShopDbContext db = new DataLayer.TfShopDbContext())
            {
                db.Database.ExecuteSqlCommandAsync("DELETE FROM ELMAH_Error WHERE TimeUtc < '" + LastMonthDate.ToString("yyyy-MM-dd") + "' DELETE FROM UnSentMessages WHERE state=1 and InsertDate < '" + LastMonthDate.ToString("yyyy-MM-dd") + "' DELETE FROM useractivities WHERE LogDateTime < '" + Last3MonthDate.ToString("yyyy-MM-dd") + "'");

                //db.Database.ExecuteSqlCommandAsync("DELETE FROM UnSentMessages WHERE state=1 and InsertDate < '" + LastMonthDate.ToString("yyyy-MM-dd") + "'");
            }

            UnitOfWorkClass uow = new UnitOfWorkClass();
            IEnumerable<Domain.EventLog> oldLog = uow.EventLogRepository.Get(x => x, x => x.LogDateTime < LastMonthDate);
            uow.EventLogRepository.Delete(oldLog.ToList());
            uow.Save();
        }
    }
    public static class UpdateUserActivity
    {
        public static void SetUserActivities()
        {
            HttpCookie aCookie = HttpContext.Current.Request.Cookies["tfactivity"];
            if (aCookie != null)
            {
                if (!String.IsNullOrEmpty(aCookie.Value))
                {
                    int CookieId = Convert.ToInt32(aCookie.Value);
                    using (DataLayer.TfShopDbContext db = new DataLayer.TfShopDbContext())
                    {
                        if (db.UserActivities.Any(x => x.Id == CookieId) && HttpContext.Current.User.Identity.IsAuthenticated)
                        {
                            string username = HttpContext.Current.User.Identity.Name.ToString();
                            db.Database.ExecuteSqlCommand("UPDATE UserActivities SET LOGON_USER=@p1 WHERE CookieId=@p2", new SqlParameter("@p1", username), new SqlParameter("@p2", CookieId));
                            var rid = db.UserActivities.Where(x => x.LOGON_USER == username && x.UserActivityRefererId!=null).FirstOrDefault();
                            if (rid != null)
                            {
                                var user = db.Users.Where(x => x.UserName == username).First();
                                if (user.UserActivityRefererId == null)
                                {
                                    user.UserActivityRefererId = rid.UserActivityRefererId;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}