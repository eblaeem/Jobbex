using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.UserJob
{
    public class UserJobSave
    {
        public int? Id { get; set; }
        public bool? IsCurrentJob { get; set; }

        [Display(Name = " تاریخ شروع")]
        public string StartDate { get; set; }

        [Display(Name = " تاریخ خاتمه")]
        public string EndDate { get; set; }
        public string Description { get; set; }

        [Display(Name = "عنوان شغل")]
        public string JobTitle { get; set; }

        [Display(Name = " نام شرکت")]
        public string CompanyName { get; set; }

        [Display(Name = " گروه شغلی")]
        public int? JobGroupId { get; set; }

        [Display(Name = "سمت")]
        public int? PositionId { get; set; }

        public int? CountryId { get; set; }

        [Display(Name = "شهر")]
        public int? CityId { get; set; }

    }
    public class UserJobSaveValidator : AbstractValidator<UserJobSave>
    {
        public UserJobSaveValidator()
        {
            RuleFor(x => x.JobGroupId).GreaterThan(0).NotEmpty().NotNull();
            RuleFor(x => x.PositionId).GreaterThan(0).NotEmpty().NotNull();
            RuleFor(x => x.CityId).GreaterThan(0).NotEmpty().NotNull();

            RuleFor(x => x.StartDate).NotEmpty().NotNull();
            RuleFor(x => x.EndDate).NotEmpty().NotNull();
            RuleFor(x => x.JobTitle).NotEmpty().NotNull();
            RuleFor(x => x.CompanyName).NotEmpty().NotNull();
        }
    }

}
