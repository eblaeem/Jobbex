using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Company
{
    public class UserCompanyResponse
    {
        public UserCompanyResponse()
        {

        }
        public UserCompanyResponse(bool isValid, string message = "")
        {
            IsValid = isValid;
            Message = message;
        }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
        public string DisplayName { get; set; }
        public int CompanyId { get; set; }
        public string SerialNumber { get; set; }
    }
}
