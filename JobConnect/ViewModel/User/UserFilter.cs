using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class UserFilter : PagingModel
    {
        [Display(Name = "نام/نام کاربری/کد ملی")]
        public int? UserId { get; set; }

        [Display(Name = "از تاریخ")]
        public string FromDate { get; set; }

        [Display(Name = "تا تاریخ")]
        public string ToDate { get; set; }

        [Display(Name = "نام کاربری")]
        public string Username { get; set; }

        [Display(Name = "نام ")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی ")]
        public string LastName { get; set; }

        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }


        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }


        [Display(Name = "وضعیت")]
        public bool? IsActive { get; set; }

        public List<Column> SelectedColumns { get; set; }

        public string Columns { get; set; }

        [Display(Name = "جستجو")]
        public string SearchString { get; set; }

        public string ContextMenuColumns { get; set; }
    }
}
