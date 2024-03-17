using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class TwoFactorModel
    {
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "کد")]
        public string Token { get; set; }
    }
    public class TwoFactorValidator : AbstractValidator<TwoFactorModel>
    {
        public TwoFactorValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().NotNull();
            RuleFor(x => x.Token).NotEmpty().NotNull();
        }
    }
}
