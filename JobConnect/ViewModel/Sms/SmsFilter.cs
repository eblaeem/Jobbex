using System.ComponentModel.DataAnnotations;

namespace ViewModel.Sms
{
    public class SmsFilter : PagingModel
    {
        [Display(Name = "از تاریخ")]
        public string FromDate { get; set; }

        [Display(Name = "تا تاریخ")]
        public string ToDate { get; set; }


        [Display(Name = "نام کاربری")]
        public string Username { get; set; }


        [Display(Name = "شماره همراه")]
        public string MobileNumber { get; set; }

        [Display(Name = "نام/نام کاربری/کد ملی")]
        public int? UserId { get; set; }

        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }

        [Display(Name = "نام،نام خانوادگی")]
        public string DisplayName { get; set; }

        [Display(Name = "متن")]
        public string Message { get; set; }

        [Display(Name = "نوع پیامک")]
        public int? SmsOutboxTypeId { get; set; }
        public List<LabelValue> SmsOutboxTypes { get; set; }

        public List<LabelValue> Customers { get; set; }

        public List<Column> SelectedColumns { get; set; }
        public string Columns { get; set; }
    }
}
