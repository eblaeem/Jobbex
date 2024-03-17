using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class CreateUserAdmin
    {

        [Display(Name = "نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "رمز عبور")]
        public string Password { get; set; }


        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }


        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }


        [Display(Name = "تکرار رمز عبور")]
        public string ConfirmPassword { get; set; }
    }

    public class CreateUserAdminValidator : AbstractValidator<CreateUserAdmin>
    {
        public CreateUserAdminValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().NotNull();
            RuleFor(x => x.LastName).NotEmpty().NotNull();
            RuleFor(x => x.Email).NotEmpty().NotNull();
            RuleFor(x => x.PhoneNumber).NotEmpty().NotNull();
            RuleFor(x => x.UserName).NotEmpty().NotNull().MinimumLength(5);
            RuleFor(x => x.Password).NotEmpty().NotNull();
            RuleFor(x => x.ConfirmPassword).NotEmpty().NotNull();
        }
    }
}
