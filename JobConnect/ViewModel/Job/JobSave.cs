using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.States;
using ViewModel.User;

namespace ViewModel.Job
{
    public class JobSave
    {
        public int Id { get; set; }

        [Display(Name = "شرکت")]
        public int CompanyId { get; set; }

        public int? UserId { get; set; }

        [Display(Name = "عنوان شغل")]
        public string Title { get; set; }

        [Display(Name = "شرح")]
        public string Description { get; set; }

        [Display(Name = "گروه شغلی")]
        public int? JobGroupId { get; set; }
        public List<LabelValue> JobGroups { get; set; }

        [Display(Name = "شهر")]
        public int? CityId { get; set; }
        public List<GroupStateResponse> Cities { get; set; }


        [Display(Name = "استان")]
        public int? StateId { get; set; }



        [Display(Name = "روزهای کاری")]
        public string WorkingDays { get; set; }

        [Display(Name = "سابقه کار")]
        public int? WorkExperienceYearId { get; set; }
        public List<LabelValue> WorkExperienceYears { get; set; }

        [Display(Name = "جنسیت")]
        public byte GenderRequired { get; set; }
        public List<LabelValue> Genders { get; set; }

        [Display(Name = "میزان تحصیلات")]
        public byte EducationLevelRequired { get; set; }
        public List<LabelValue> EducationLevels { get; set; }

        [Display(Name = "وضعیت سربازی")]
        public byte MilitaryStateRequired { get; set; }
        public List<LabelValue> MilitaryStates { get; set; }

        [Display(Name = "وضعیت")]
        public bool Status { get; set; }


        [Display(Name ="ورود به سایت")]
        public bool RequiredLoginToSite { get; set; }

        [Display(Name = "مشخصات تکمیلی")]
        public bool RequiredInformation { get; set; }

        [Display(Name = "سوابق تحصیلی/آموزشی")]
        public bool RequiredEducation { get; set; }

        [Display(Name = " سوابق شغلی ")]
        public bool RequiredJob { get; set; }

        [Display(Name = " اطلاعات زبان ها ")]
        public bool RequiredLanguage { get; set; }

        [Display(Name = " مهارت های شغلی ")]
        public bool RequiredSkills { get; set; }

        [Display(Name = " الویت های شغلی ")]
        public bool RequiredPriorities { get; set; }


        [Display(Name = "حقوق درخواستی")]
        public int? SalaryTypeId { get; set; }
        public List<LabelValue> SalaryTypes { get; set; }

        [Display(Name = "نوع همکاری")]
        public int? ContractTypeId { get; set; }
        public List<LabelValue> ContractTypes { get; set; }
    }
    public class JobSaveValidator : AbstractValidator<JobSave>
    {
        public JobSaveValidator()
        {
            RuleFor(x => x.Title).NotEmpty().NotNull();
            RuleFor(x => x.Description).NotEmpty().NotNull();

            RuleFor(x => x.JobGroupId).GreaterThan(0).NotEmpty().NotNull();
            RuleFor(x => x.CityId).GreaterThan(0).NotEmpty().NotNull();
            RuleFor(x => x.StateId).GreaterThan(0).NotEmpty().NotNull();
            RuleFor(x => x.WorkExperienceYearId).GreaterThan(0).NotEmpty().NotNull();
            RuleFor(x => x.EducationLevelRequired).NotEmpty().NotNull();
            RuleFor(x => x.MilitaryStateRequired).NotEmpty().NotNull();
            RuleFor(x => x.SalaryTypeId).NotEmpty().NotNull();
            RuleFor(x => x.ContractTypeId).NotEmpty().NotNull();
        }
    }
}
