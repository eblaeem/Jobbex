using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class ChangePasswordSave
    {
        [Display(Name = "رمز عبور فعلی")]
        public string CurrentPassword { get; set; }

        [Display(Name = "رمز عبور جدید")]
        public string NewPassword { get; set; }
        [Display(Name = "تکرار رمز عبور جدید")]
        public string ConfirmPassword { get; set; }
    }
    public class ChangePasswordSaveValidator : AbstractValidator<ChangePasswordSave>
    {
        public ChangePasswordSaveValidator()
        {
            RuleFor(x => x.CurrentPassword).NotEmpty().NotNull();
            RuleFor(x => x.NewPassword).NotEmpty().NotNull();
            RuleFor(x => x.ConfirmPassword).NotEmpty().NotNull();
        }
    }
}
