using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Company;
using ViewModel.Job;
using ViewModel.States;

namespace ViewModel.Dashboard
{
    public class DashboardResponse
    {
        public List<GroupStateResponse> GroupStates { get; set; }
        public List<JobResponse> Latest { get; set; }
        public List<LabelValue> PopularTags { get; set; }
        public List<CompanyResponse> PopularCompanies { get; set; }
        public int UserCount { get; set; }
        public int JobCount { get; set; }
        public int ViewsCount { get; set; }
        public List<LabelValue> Groups { get; set; }
    }
}
