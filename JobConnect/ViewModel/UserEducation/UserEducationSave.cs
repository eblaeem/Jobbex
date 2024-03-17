using FluentValidation;

namespace ViewModel.UserEducation
{
    public class UserEducationSave
    {
        public int? Id { get; set; }
        public string Field { get; set; }
        public string UniversityName { get; set; }
        public int? DegreeId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public decimal? Score { get; set; }
        public string Description { get; set; }
    }
    public class UserEducationSaveValidator : AbstractValidator<UserEducationSave>
    {
        public UserEducationSaveValidator()
        {
            RuleFor(x => x.Field).NotEmpty().NotNull();
            RuleFor(x => x.UniversityName).NotEmpty().NotNull();
            RuleFor(x => x.DegreeId).GreaterThan(0).NotEmpty().NotNull();
            RuleFor(x => x.StartDate).NotEmpty().NotNull();
            RuleFor(x => x.EndDate).NotEmpty().NotNull();
        }
    }
}
