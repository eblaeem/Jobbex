using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.UserRecommended
{
    public class UserRecommendedResponse
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string CityName { get; set; }
        public int? CityId { get; set; }
        public int? JobGroupId { get; set; }
        public int? SalaryTypeId { get; set; }
        public int? ContractTypeId { get; set; }
        public string ZoneName { get; set; }
        public string SalaryTypeName { get; set; }
        public string WorkExperienceYearTitle { get; set; }
        public string ContractTypeName { get; set; }
        public List<LabelValue> JobSkill { get; set; }
        public string SalaryTypeTitle { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeString { get; set; }
        public DateTime ExpiredDateTime { get; set; }
        public string WorkingDays { get; set; }
        public string GenderRequiredTitle { get; set; }
        public string EducationLevelRequiredTitle { get; set; }
        public string MilitaryStateRequiredTitle { get; set; }
        public bool JobStatus { get; set; }


        public string CompanyName { get; set; }
        public int? CompanyCreatedYear { get; set; }
        public string CompanyOrganizationSize { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyServiceAndProducs { get; set; }
        public decimal? CompanyRate { get; set; }


        public int TotalRowCount { get; set; }
        public int? AttachmentLogoId { get; set; }
        public string AttachmentLogoString { get; set; }

        public int? RankScore { get; set; }
        public List<JobBenefit> JobBenefits { get; set; } = new List<JobBenefit>();
    }
}
