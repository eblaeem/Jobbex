using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Dashboard
{
    public class DashboardJobResponse
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string JobDescription { get; set; }
        public int UserRequestCount { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeString { get; set; }
        public string JobGroupName { get; set; }
    }

}
