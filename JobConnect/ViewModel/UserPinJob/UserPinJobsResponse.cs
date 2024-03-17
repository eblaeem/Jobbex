using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.UserPinJob
{
    public class UserPinJobsResponse
    {
        public int JobId { get; set; }
        public int UserId { get; set; }
        public string JobTitle { get; set; }
        public bool Status { get; set; }
        public string StatusName { get; set; }
        public int CityId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CityName { get; set; }
        public int ContractId { get; set; }
        public string ContractName { get; set; }
        public int StateId { get; set; }
        public int JobGroupId { get; set; }
        public string JobGroupName { get; set; }
        public string StatePersianName { get; set; }
        public int SalaryTypeId { get; set; }
        public string SalaryTypeName { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeString { get; set; }
        public string CompanyLogo { get; set; }
        public DateTime ExpireDateTime { get; set; }
        public int? AttachmentLogoId { get; set; }
        public int TotalRowCount { get; set; }
    }
}
