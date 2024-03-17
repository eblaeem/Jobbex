using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.JobRequest
{
    public class JobRequestSave
    {
        public int JobId { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public IFormFile File { get; set; }
    }
}
