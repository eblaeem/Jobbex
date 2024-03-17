namespace Entities
{
    public class Job
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? JobGroupId { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set;}
        public DateTime DateTime { get; set; }
        public string WorkingDays { get; set; }
        public int? WorkExperienceYearId { get; set; }
        public byte GenderRequired { get; set; }
        public byte EducationLevelRequired { get; set; }
        public byte MilitaryStateRequired { get; set; }
        public DateTime ExpiredDateTime { get; set; }
        public bool Status { get; set; }
        public int? SalaryTypeId { get; set; }
        public int? ContractTypeId { get; set; }

        public bool? RequiredLoginToSite { get; set; }
        public bool? RequiredInformation { get; set; }
        public bool? RequiredEducation { get; set; }
        public bool? RequiredJob { get; set; }
        public bool? RequiredLanguage { get; set; }
        public bool? RequiredSkills { get; set; }
        public bool? RequiredPriorities { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<UserPinJob> UserPinJobs{ get; set; }
    }
}
