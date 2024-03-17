using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class JobRequestState
    {
        public int Id { get; set; }
        public int? JobRequestId { get; set; }

        public int? TypeId { get; set; }

        public virtual JobRequested JobRequested { get; set; }

        public DateTime DateTime { get; set; }

        public string Description { get; set; }
    }
}
