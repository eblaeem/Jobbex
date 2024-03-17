using System.ComponentModel.DataAnnotations;

namespace ViewModel.AccessCode
{
    public class AccessCodeFilter : PagingModel
    {
        public int Id { get; set; }
        public string Columns { get; set; }

        [Display(Name = "عنوان")]
        public string Name { get; set; }

        [Display(Name = "شناسه")]
        public int? Number { get; set; }
    }
}
