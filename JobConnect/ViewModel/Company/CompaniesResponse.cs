using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Company
{
    public class CompaniesResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string JobGroupName { get; set; }
        public int ActiveJobCount { get; set; }
        public string OrganizationSizeName { get; set; }
        public string WebSite { get; set; }

        public int? AttachmentBackgroundId { get; set; }
        public string CompanyBackground { get; set; }


        public string CompanyLogo { get; set; }
        public int? AttachmentLogoId { get; set; }

        public int? TotalRowCount { get; set; }
        public int UserId { get; set; }
    }
}
