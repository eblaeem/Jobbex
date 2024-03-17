using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.States
{
    public class GroupStateResponse
    {
        public string Label { get; set; }
        public int? Data { get; set; }
        public List<LabelValue> Options { get; set; }
    }
}
