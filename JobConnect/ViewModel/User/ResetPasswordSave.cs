using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class ResetPasswordSave
    {
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "شناسه")]
        public string Token { get; set; }

        [Display(Name = "رمز عبور جدید")]
        public string NewPassword { get; set; }

        [Display(Name = "تکرار رمز عبور جدید")]
        public string ConfirmNewPassword { get; set; }
    }
    public class ResetPasswordSaveValidator : AbstractValidator<ResetPasswordSave>
    {
        public ResetPasswordSaveValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().NotNull();
            RuleFor(x => x.Token).NotEmpty().NotNull();
            RuleFor(x => x.NewPassword).NotEmpty().NotNull();
            RuleFor(x => x.ConfirmNewPassword).NotEmpty().NotNull();
        }
    }
}
