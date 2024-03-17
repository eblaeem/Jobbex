using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class ConfirmResetPasswordSave
    {
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "شناسه")]
        public string Token { get; set; }
    }
    public class ConfirmResetPasswordSaveValidator : AbstractValidator<ConfirmResetPasswordSave>
    {
        public ConfirmResetPasswordSaveValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().NotNull();
            RuleFor(x => x.Token).NotEmpty().NotNull();
        }
    }
}
