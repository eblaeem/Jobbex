using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Company
{
    public class CompanyJobResponse
    {
        public int JobId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string JobDescription { get; set; }
        public int? CityId { get; set; }
        public string CityName { get; set; }


        public int? ContractTypeId { get; set; }
        public string ContractTypeName { get; set; }

        public string JobTitle { get; set; }

        public int? JobGroupId { get; set; }
        public string JobGroupName { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeString { get; set;}

        public int? SalaryRequestedId { get; set; }
        public string SalaryRequestedName { get; set; }

        public int? WorkExperienceYearId { get; set; }
        public string WorkExperienceYearName { get; set; }

        public int? TotalRowCount { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public int JobRequestedCount { get; set; }
        public bool Status { get; set; }
    }
}
