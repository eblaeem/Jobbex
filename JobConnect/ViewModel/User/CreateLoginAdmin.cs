using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class CreateLoginAdmin
    {
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "کد امنیتی")]
        public string Captcha { get; set; }

        public string ClientGuid { get; set; }

    }
    public class CreateLoginAdminValidator : AbstractValidator<CreateLoginAdmin>
    {
        public CreateLoginAdminValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().NotNull();
            RuleFor(x => x.Password).NotEmpty().NotNull();
            RuleFor(x => x.Captcha).NotEmpty().NotNull().MinimumLength(4);
        }
    }
}
