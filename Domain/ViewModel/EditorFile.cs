using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel
{
    public class EditorFile
    {
        public EditorFile()
        {

        }
        [Required(ErrorMessage = "اجباری")]
        [Display(Name = "نام فایل")]
        public string Title { get; set; }

        [Display(Name = "فایل فایل")]
        public string FileName { get; set; }

        public string fileType { get; set; }
    }
}
