using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Company
{
    public class CompanyFilter : PagingModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<int> JobGroups { get; set; }
    }
}
