using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.UserLanguage
{
    public class UserLanguageSave
    {
        public int? Id { get; set; }

        [Display(Name ="نام زبان")]
        public int LanguageId { get; set; }

        [Display(Name = "سطح تسلط")]
        public int LevelId { get; set; }
    }
    public class UserLanguageSaveValidator : AbstractValidator<UserLanguageSave>
    {
        public UserLanguageSaveValidator()
        {
            RuleFor(x => x.LanguageId).GreaterThan(0).NotEmpty().NotNull();
            RuleFor(x => x.LevelId).GreaterThan(0).NotEmpty().NotNull();
    
        }
    }
}
