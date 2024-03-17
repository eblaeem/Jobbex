using FluentValidation;
using System.ComponentModel.DataAnnotations;
using ViewModel.States;
using ViewModel.UserEducation;
using ViewModel.UserJob;
using ViewModel.UserLanguage;
using ViewModel.UserPriority;
using ViewModel.UserSoftwareSkill;

namespace ViewModel.User
{
    public class UserProfile
    {
        [Display(Name = "نام")]
        public string FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        public string LastName { get; set; }

        [Display(Name = "شماره همراه")]
        public string PhoneNumber { get; set; }

        [Display(Name = "ایمیل")]
        public string Email { get; set; }


        public string DisplayName { get; set; }
        public string LastLoggedIn { get; set; }
        public string NationalCode { get; set; }
        public string UserName { get; set; }

        public string UserProfileImage { get; set; }
        public int? UserProfileImageId { get; set; }
        public string Description { get; set; }


        public int? CityId { get; set; }
        public string CityName { get; set; }

        public int? StateId { get; set; }


        public byte? GenderId { get; set; }
        public string GenderName { get; set; }
        public int? SalaryRequestedId { get; set; }
        public string SalaryRequestedName { get; set; }
        public int? ExpectedJobId { get; set; }

        public byte? MaritalStateId { get; set; }
        public string MaritalStateName { get; set; }
        public byte? MilitaryStateId { get; set; }
        public string MilitaryStateStateName { get; set; }

        public string BirthDate { get; set; }
        public int? Age { get; set; }
        public DateTime? GeoBirthDate { get; set; }
        public string Address { get; set; }

        public string ChangePasswordDateTimeString { get; set; }
        public List<UserTokenResponse> UserTokens { get; set; } = new List<UserTokenResponse>();
        public int? PersonTypeId { get; set; }




        public List<GroupStateResponse> Cites { get; set; }
        public List<LabelValue> SalaryRequesteds { get; set; }
        public List<LabelValue> Genders { get; set; }
        public List<LabelValue> MaritalStatus { get; set; }
        public List<LabelValue> MilitaryStatus { get; set; }
        public List<LabelValue> Degrees { get; set; }

        public List<LabelValue> Languages { get; set; }
        public List<LabelValue> LanguageLevels { get; set; }

        public List<LabelValue> JobPositions { get; set; }
        public List<LabelValue> JobGroups { get; set; }

        public List<LabelValue> Skills { get; set; }
        public List<LabelValue> SkillLevels { get; set; }
        public List<LabelValue> ContractTypes { get; set; }
        public List<LabelValue> JobBenefits { get; set; }

        public List<UserEducationResponse> UserEducations { get; set; }
        public List<UserJobResponse> UserJobs { get; set; }
        public List<UserLanguageResponse> UserLanguages { get; set; }
        public List<UserSoftwareSkillResponse> UserSkills { get; set; }
        public UserPriorityResponse UserPriorities { get; set; }
    }
    public class UserProfileSaveValidator : AbstractValidator<UserProfile>
    {
        public UserProfileSaveValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().NotNull();
            RuleFor(x => x.LastName).NotEmpty().NotNull();
            RuleFor(x => x.PhoneNumber).NotEmpty().NotNull();
        }
    }
}
