using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.JobRequest
{
    public class JobRequestResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserAvatar { get; set; }
        public int? AttachmentResumehId { get; set; }

        public string StatusName { get; set; }
        public int? StateId { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeString { get; set; }

        public string UserCityName { get; set; }
        public int? TotalRowCount { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

    }
}
