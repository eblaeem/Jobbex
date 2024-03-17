using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Job;

namespace ViewModel.Company
{
    public class CompanyDetailResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string JobGroupName { get; set; }
        public int ActiveJobCount { get; set; }
        public string OrganizationSizeName { get; set; }
        public string WebSite { get; set; }


        public int JobCount { get; set; }
        public string CompanyBackground { get; set; }
        public string CompanyLogo { get; set; }

        public List<JobResponse> ActiveJobs { get; set; } = new List<JobResponse>();
        public List<JobResponse> ExpiredJobs { get; set; } = new List<JobResponse>();
    }
}
