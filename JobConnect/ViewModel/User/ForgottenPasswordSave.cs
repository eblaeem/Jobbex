using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class ForgottenPasswordSave
    {
        //[Display(Name = "نام کاربری")]
        //public string Email { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }

        //[Display(Name ="شماره همراه")]
        //public string MobileNumber { get; set; }

        [Display(Name = "کد امنیتی")]
        public string Captcha { get; set; }

        public string ClientGuid { get; set; }
    }
    public class ForgottenPasswordSaveValidator : AbstractValidator<ForgottenPasswordSave>
    {
        public ForgottenPasswordSaveValidator()
        {
            RuleFor(x => x.Email).NotEmpty().NotNull();
            RuleFor(x => x.Captcha).NotEmpty().NotNull().MinimumLength(4);
            RuleFor(x => x.ClientGuid).NotEmpty().NotNull();
        }
    }
    public class ForgottenPasswordResponse : ResponseBase
    {
        public ForgottenPasswordResponse(bool isValid, string message = "")
        {
            IsValid = isValid;
            Message = message;
        }
        public string MobileNUmber { get; set; }
        public string Email { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string NationalCode { get; set; }
    }
}
