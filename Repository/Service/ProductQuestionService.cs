using DataLayer;
using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Repository.Service
{
    public class ProductQuestionService : GenericRepository<ProductQuestion>
    {
        private readonly ProductService _productService;
        public ProductQuestionService(TfShopDbContext context, ProductService productService) : base(context)
        {
            this._productService = productService;
        }



        /// <summary>
        /// نمایش لیست پرسش و پاسخ ها در صفحه محصول تب نظرات
        /// </summary>
        /// <param name="productid"></param>
        /// <returns></returns>
        public IQueryable<ProductFAQList> GetProductFAQ(int productid)
        {
            return GetByReturnQueryable(x => new ProductFAQList
            {
                Id = x.Id,
                FullName = !String.IsNullOrEmpty(x.FakeUserFullName) ? x.FakeUserFullName : x.User.FirstName,
                InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(x.InsertDate),
                InsertTime = x.InsertDate.ToShortTimeString(),
                Text = x.Message,
                UserAvatar = x.User.Avatar,
                UserGender = x.User.Gender,
                like = x.Like,
                unlike = x.UnLike,
                ChildComment = x.ChildComment,
                UserId = x.UserId,
                parrentid = x.ParrentId


            }, x => x.ProductId == productid && x.IsActive, null, "ChildComment.attachments,User");
        }



        public IEnumerable<ProductFAQListvm> GetProductFAQv2(int productid)
        {
            return Get(x => new ProductFAQListvm
            {
                Id = x.Id,
                FullName = !String.IsNullOrEmpty(x.FakeUserFullName) ? x.FakeUserFullName : x.User.FirstName,
                InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(x.InsertDate),
                InsertTime = x.InsertDate.ToShortTimeString(),
                Text = x.Message,
                UserAvatar = x.User.Avatar.HasValue ? x.User.Avatarattachment.FileName : "",
                UserGender = x.User.Gender,
                Useful = x.Like,
                UnUseful = x.UnLike,
                ChildComment = x.ChildComment.Select(s => new ProductFAQListvm()
                {
                    Id = s.Id,
                    FullName = !String.IsNullOrEmpty(s.FakeUserFullName) ? s.FakeUserFullName : s.User.FirstName ,
                    UserId = s.UserId,
                    parrentid = s.ParrentId,
                    InsertDate = CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToLongShamsi(s.InsertDate),
                    InsertTime = s.InsertDate.ToShortTimeString(),
                    Text = s.Message,
                    UserAvatar = s.User.Avatar.HasValue ? s.User.Avatarattachment.FileName : "",
                    UserGender = s.User.Gender,
                    Useful = s.Like,
                    UnUseful = s.UnLike,
                    AdminAnswer=s.AdminAnswer
                }),
                UserId = x.UserId,
                parrentid = x.ParrentId


            }, x => x.ProductId == productid && x.IsActive, null, "attachments,User.Avatarattachment,ChildComment.attachments,ChildComment.User.Avatarattachment");
        }
        public IEnumerable<ProductFAQGoogleList> GetProductGoogleFAQ(int productid)
        {
            return GetQueryList().Include("ChildComment").Include("User").AsNoTracking().Where(x => x.ParrentId == null && x.ProductId == productid && x.IsActive && x.ChildComment.Any()).OrderByDescending(x => x.Id).Skip(() => 0).Take(() => 5).Select(x => new ProductFAQGoogleList
            {
                @type = "Question",
                name = x.Message,
                acceptedAnswer = new acceptedAnswer { type = "Answer", text = x.ChildComment.Any() ? x.ChildComment.FirstOrDefault().Message : "----" }


            }).AsEnumerable();
        }


        public bool SetUsefulQuestion(int commentid)
        {
            var pc = Get(x => x, x => x.Id == commentid).FirstOrDefault();
            if (pc != null)
            {
                pc.Like++;
                Update(pc);
                return true;
            }
            else
                return false;
        }
        public bool SetUnusefulQuestion(int commentid)
        {
            var pc = Get(x => x, x => x.Id == commentid).FirstOrDefault();
            if (pc != null)
            {
                pc.UnLike++;
                Update(pc);
                return true;
            }
            else
                return false;
        }




    }
}
