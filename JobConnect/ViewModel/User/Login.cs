using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class Login
    {
        /// <summary>
        /// The last name of the author
        /// </summary>
        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [Display(Name = "کد امنیتی")]
        public string Captcha { get; set; }

        public string ClientGuid { get; set; }
    }
    public class LoginValidator : AbstractValidator<Login>
    {
        public LoginValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().NotNull();
            RuleFor(x => x.Password).NotEmpty().NotNull();
            //RuleFor(x => x.Password).Matches(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$");
            RuleFor(x => x.Captcha).NotEmpty().NotNull().MinimumLength(4);
        }
    }
}
