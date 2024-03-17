using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Job
{
    public class JobResponse
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; }
        public DateTime? ExpireDateTime { get; set; }

        public string CompanyName { get; set; }
        public decimal? CompanyRate { get; set; }
        public int? AttachmentLogoId { get; set; }
        public string CompanyLogo { get; set; }

        public string JobTitle { get; set; }

        public int JobGroupId { get; set; }
        public string JobGroupName { get; set; }

        public int? WorkExperienceYearId { get; set; }
        public string WorkExperienceYearName { get; set; }

        public int? CityId { get; set; }
        public string CityName { get; set; }
        public string ZoneName { get; set; }

        public string SalaryRequestedName { get; set; }
        public int? SalaryRequestedId { get; set; }
        public int? ContractTypeId { get; set; }
        public string ContractTypeName { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeString { get; set; }

        public int? TotalRowCount { get; set; }
    }
}
