using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Job
{
    public class JobDetailResponse
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string CityName { get; set; }
        public int CityId { get; set; }
        public string ZoneName { get; set; }
        public string CompanyName { get; set; }
        public string WebSite { get; set; }
        public decimal? CompanyRate { get; set; }
        public int? AttachmentLogoId { get; set;     }
        public string SalaryRequestedName { get; set; }
        public string SalaryTypeTitle { get; set; }

        public int SalaryTypeId { get; set; }
        public string ContractTypeName { get; set; }
        public int ContractTypeId { get; set; }
        public string CompanyLogo { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeString { get; set; }

        public string WorkExperienceYearTitle { get; set; }
        public byte WorkExperienceYearId { get; set; }
        public List<string> ContractTypes { get; set; }
        public List<LabelValue> JobSkill { get; set; }
        public string WorkingDays { get; set; }
        public string Description { get; set; }
        public string GenderRequiredTitle { get; set; }
        public byte GenderRequiredId { get; set; }
        public string EducationLevelRequiredTitle { get; set; }
        public byte EducationLevelRequiredId { get; set; }
        public string MilitaryStateRequiredTitle { get; set; }
        public byte MilitaryStateRequiredId { get; set; }

        public int? CompanyId { get; set; }
        public int? CompanyCreatedYear { get; set; }
        public int? CompanyOrganizationSize { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyServiceAndProducs { get; set; }

        public int JobGroupId { get; set; }
        public string JobGroupName { get; set; }
        public bool Status { get; set; }

        public bool RequiredLoginToSite { get; set; }
        public bool RequiredInformation { get; set; }
        public bool RequiredEducation { get; set; }
        public bool RequiredJob { get; set; }
        public bool RequiredLanguage { get; set; }
        public bool RequiredSkills { get; set; }
        public bool RequiredPriorities { get; set; }
        public List<string> CompanyAttachments { get; set; } = new List<string>();
    }
}
