using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.UserPriority
{
    public class UserPriorityResponse
    {
        public List<int> Cities { get; set; }
        public List<int> Groups { get; set; }
        public List<int> ContractTypes { get; set; }
        public List<int> Benefits { get; set; }

        public List<string> GroupsName { get; set; }
        public int? SalaryRequestedId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
    }
}
