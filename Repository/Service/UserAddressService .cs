using DataLayer;
using Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class UserAddressService : GenericRepository<UserAddress>
    {
        public UserAddressService(TfShopDbContext context) : base(context)
        {

        }
        /// <summary>
        /// متد بررسی موجود بودن آدرس در لیست ادرس های کاربر
        /// </summary>
        /// <param name="addressId">کد آدرس</param>
        /// <param name="userid">کد کاربر</param>
        /// <returns></returns>

        public int? checkUserUserAddress(int addressId, string userid)
        {
            return Get(x => x.Id, x => x.Id == addressId && x.UserId == userid, null, "", 0, 0, true).FirstOrDefault();

        }
        /// <summary>
        /// حذف آدرس کاربر
        /// </summary>
        /// <param name="id">ردیف آدرس کاربر</param>
        /// <returns></returns>
        public async Task removeUserAddress(int id)
        {
            Delete(dbSet.Find(id));
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// افزودن آدرس به آدرس های کاربر
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns></returns>
        public async Task addUserAddress(UserAddress useraddress)
        {
            Insert(useraddress);
            await SaveAllowingEmptyNumber(useraddress);
        }


        /// <summary>
        /// ویرایش آدرس کاربر
        /// </summary>
        /// <param name="useraddress"></param>
        /// <returns></returns>
        public async Task EditUserAddress(UserAddress useraddress)
        {
            Update(useraddress);
            await SaveAllowingEmptyNumber(useraddress);
        }

        /// <summary>
        /// پلاک اختیاری است (فرمِ آدرسِ سایت)، ولی در Domain رویِ AddressNumber هنوز [Required] هست - برداشتنش مدلِ EF را
        /// عوض می‌کند و auto-migration راه می‌اندازد. برایِ همین آدرسِ بی‌پلاک با رشته‌ی خالی و فقط در همین ذخیره بدونِ
        /// اعتبارسنجیِ EF ذخیره می‌شود (نمایش‌ها همه IsNullOrEmpty را چک می‌کنند). بقیه‌ی فیلدها را فرم و ستون‌هایِ دیتابیس می‌پایند.
        /// </summary>
        private async Task SaveAllowingEmptyNumber(UserAddress useraddress)
        {
            if (!string.IsNullOrWhiteSpace(useraddress.AddressNumber))
            {
                await context.SaveChangesAsync();
                return;
            }
            useraddress.AddressNumber = "";
            context.Configuration.ValidateOnSaveEnabled = false;
            try
            {
                await context.SaveChangesAsync();
            }
            finally
            {
                context.Configuration.ValidateOnSaveEnabled = true;
            }
        }

        public async Task<bool> UserHasAnyAddress(string userid)
        {
            //var ssss = await context.Users.Include(x => x.UserAddresses).Where(x => x.UserAddresses.Any(s => s.UserId == userid) || (x.Id == userid && x.Address != null && x.CityId.HasValue && x.Address != "")).FirstOrDefaultAsync();
            return await context.Users.Include(x => x.UserAddresses).AnyAsync(x => x.UserAddresses.Any(s => s.UserId == userid) || (x.Id == userid && x.Address != null && x.CityId.HasValue && x.Address != ""));
        }
        public async Task<bool> UserHasAnyPersonalInfo(string userid)
        {
            //var ssss = await context.Users.Include(x => x.UserAddresses).Where(x => x.UserAddresses.Any(s => s.UserId == userid) || (x.Id == userid && x.FirstName != null && x.LastName != null && x.FirstName != "" && x.LastName != "")).FirstOrDefaultAsync();

            return await context.Users.Include(x => x.UserAddresses).AnyAsync(x =>(x.PhoneNumberConfirmed && x.Disable == false ) && ( x.UserAddresses.Any(s => s.UserId == userid) || (x.Id == userid && x.FirstName != null && x.LastName != null && x.FirstName != "" && x.LastName != "")));
        }

    }
}
