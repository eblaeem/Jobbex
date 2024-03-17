using FluentValidation;
using System.ComponentModel.DataAnnotations;
using ViewModel.User;

namespace ViewModel.UserSoftwareSkill
{
    public class UserSoftwareSkillSave
    {
        public int? Id { get; set; }

        [Display(Name = "مهارت و تخصص ها")]
        public int SoftwareSkillId { get; set; }

        [Display(Name = "سطح آشنایی")]
        public int LevelId { get; set; }
    }
    public class UserSoftwareSkillSaveValidator : AbstractValidator<UserSoftwareSkillSave>
    {
        public UserSoftwareSkillSaveValidator()
        {
            RuleFor(x => x.SoftwareSkillId).GreaterThan(0).NotEmpty().NotNull();
            RuleFor(x => x.LevelId).GreaterThan(0).NotEmpty().NotNull();
        }
    }
}
