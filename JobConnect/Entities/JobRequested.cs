using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class JobRequested
    {
        public int Id { get; set; }

        public int JobId { get; set; }
        public virtual Job Job { get; set; }

        public int? UserId { get; set; }
        public virtual User User { get; set; }

        public int? AttachmentResumehId { get; set; }
        public int? JobRequestedStatusId { get; set; }

        public string PhoneNumber { get; set; }
        public string DisplayName { get; set; }
        public DateTime DateTime { get; set; }
    }
}
