using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Job;

namespace ViewModel.Dashboard
{
    public class CompanyDashboard
    {
        public int JobCount { get; set; }
        public int ActiveJobCount { get; set; }
        public int UserRequestCount { get; set; }
        public int UserRequestTodayCount { get; set; }
        public List<DashboardJobResponse> ActiveJobs { get; set; } = new List<DashboardJobResponse>();
    }
}
