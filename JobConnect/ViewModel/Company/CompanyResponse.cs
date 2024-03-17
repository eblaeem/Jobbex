using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Company
{
    public class CompanyResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int JobCount { get; set; }

        public int? AttachmentLogoId { get; set; }
        public string CompanyLogo { get; set; }



    }
}
