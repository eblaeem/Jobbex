using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Company;
using ViewModel.States;

namespace ViewModel.Job
{
    public class JobResponseOption
    {
        public List<LabelValue> States { get; set; }
        public List<JobResponse> Latest { get; set; }
        public List<LabelValue> PopularTags { get; set; }
        public List<LabelValue> Groups { get; set; }
        public List<LabelValue> ContractTypes { get; set; }
        public List<LabelValue> SalaryRequesteds { get; set; }
        public List<LabelValue> WorkExperienceYears { get; set; }
    }
}
