using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TfShop.Infrastructure.DbSql
{
    public static class SqlManager
    {
        public static List<int> GetAllSubCat(int CatId)
        {
            DataLayer.TfShopDbContext db = new DataLayer.TfShopDbContext();
            try
            {
                List<int> subCatIds = db.Database.SqlQuery<int>(string.Format("select * from GetAllSubCat({0})", CatId)).ToList();
                return subCatIds;
            }
            catch (Exception x)
            {
                return null;
            }
            finally
            {
                db.Dispose();
            }
        }
    }
}