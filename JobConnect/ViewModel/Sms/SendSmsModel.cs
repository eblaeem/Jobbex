using System.ComponentModel.DataAnnotations;
using FluentValidation;
namespace ViewModel.Sms
{
    public class SendSmsModel
    {
        [Display(Name = "شماره همراه")]
        public string SelectedPhoneNumbers { get; set; }

        [Display(Name = "متن")]
        public string Message { get; set; }

        public string SmsNumber { get; set; }
        public List<LabelValue> Items { get; set; } = new List<LabelValue>();
    }
    public class SendSms
    {
        [Display(Name = "شماره همراه")]
        public List<LabelValue> SelectedPhoneNumbers { get; set; }

        [Display(Name = "متن")]
        public string Message { get; set; }
    }
    public class SendSmsValidator : AbstractValidator<SendSms>
    {
        public SendSmsValidator()
        {
            RuleFor(x => x.Message).NotEmpty().NotNull();
        }
    }
}
