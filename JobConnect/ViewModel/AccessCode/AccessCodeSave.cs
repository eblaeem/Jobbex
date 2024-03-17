using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.AccessCode
{
    public class AccessCodeSave
    {
        public int? Id { get; set; }


        [Display(Name = "نام")]
        public string Name { get; set; }

        [Display(Name = "شناسه")]
        public int Number { get; set; }

        public bool IsActive { get; set; }
    }
    public class AccessCodeSaveValidator : AbstractValidator<AccessCodeSave>
    {
        public AccessCodeSaveValidator()
        {
            RuleFor(x => x.Name).NotEmpty().NotNull();
            RuleFor(x => x.Number).NotEmpty().NotNull();
        }
    }
}
