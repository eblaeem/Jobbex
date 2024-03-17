using System.ComponentModel.DataAnnotations;

namespace ViewModel.Audit
{
    public class AuditFilter : PagingModel
    {
        public int? Id { get; set; }
        public string TableName { get; set;}

        [Display(Name = "از تاریخ")]
        public string FromDate { get; set; }
        [Display(Name = "تا تاریخ")]
        public string ToDate { get; set; }

        public string SearchString { get; set; }
    }
}
