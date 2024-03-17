using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.UserPriority
{
    public class UserPrioritySave
    {
        public int Id { get; set; }
        public List<int> Cities { get; set; }
        public List<int> Groups { get; set; }
        public List<int> ContractTypes { get; set; }
        public List<int> Benefits { get; set; }
        public int? SalaryRequestedId { get; set; }
    }
}
