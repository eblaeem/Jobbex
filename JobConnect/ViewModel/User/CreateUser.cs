using FluentValidation;
using System.ComponentModel.DataAnnotations;
using ViewModel.Attachment;
namespace ViewModel.User
{
    public class CreateUser
    {

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "رمز عبور")]
        public string Password { get; set; }


        [Display(Name = "تکرار رمز عبور")]
        public string ConfirmPassword { get; set; }



        [Display(Name = "کد امنیتی")]
        public string Captcha { get; set; }

        public string ClientGuid { get; set; }

        [Display(Name = "نام نام خانوادگی")]
        public string DisplayName { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }



        [Display(Name ="نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }
    }

    public class CreateUserValidator : AbstractValidator<CreateUser>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().NotNull();
            RuleFor(x => x.LastName).NotEmpty().NotNull();
            RuleFor(x => x.PhoneNumber).NotEmpty().NotNull();

            RuleFor(x => x.UserName).NotEmpty().NotNull().MinimumLength(5);
            RuleFor(x => x.Password).NotEmpty().NotNull();
            RuleFor(x => x.Captcha).NotEmpty().NotNull().MinimumLength(4);
        }
    }

}
