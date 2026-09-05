using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Data;
using Domain;

namespace Domain.ViewModels
{
    public class ProductAddComment
    {
        #region Ctor
        public ProductAddComment()
        {

        }
        #endregion

        #region Properties

        public ProductComment ProductComment { get; set; }
        public IEnumerable<ProductRank> productRanks { get; set; }
        public IEnumerable<ProductAdvantageTitle> ProductAdvantageTitless { get; set; }
        public IEnumerable<ProductDisAdvantageTitle> ProductDisAdvantageTitles { get; set; }
        public string breadCrumb { get; set; }
        public ProductItem productItem { get; set; }
        public bool IsBuy { get; set; }
        public bool AllowAddComment { get; set; }
        #endregion
    }
    public class ProductAddCommentv2
    {
        public int ProductId { get; set; }
        public bool commenter { get; set; }
        public ProductCommentvm ProductComment { get; set; }
        public IEnumerable<ProductRankvm> productRanks { get; set; }
        public IEnumerable<ProductAdvantageTitle> ProductAdvantageTitless { get; set; }
        public IEnumerable<ProductDisAdvantageTitle> ProductDisAdvantageTitles { get; set; }
        public ProductItemvm productItem { get; set; }
        public bool IsBuy { get; set; }
        public bool AllowAddComment { get; set; }
    }
    public class attachmentvm
    {
        public Guid id { get; set; }
        public string filename { get; set; }
    }
    public class ProductCommentvm
    {
        public int ProductId { get; set; }
        public bool IsTemp { get; set; }
        public string Text { get; set; }
        public IEnumerable<attachmentvm> attachments { get; set; }
    }
    public class ProductRankvm
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class ProductCommentAddForm : Object
    {
        public ProductCommentAddForm()
        {

        }
        public ProductCommentAddForm(int id)
        {

            ProductId = id;
        }
      


        public int? Id { get; set; }


        public int ProductId { get; set; }

        public int? OrderRowId { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }

        public bool IsTemp { get; set; }

        public int? Satisfaction { get; set; }

        public bool IsBuy { get; set; }

 

        public bool fakeComment { get; set; }
        public string fakeName { get; set; }
        public string fakeFamily { get; set; }
    }

}
