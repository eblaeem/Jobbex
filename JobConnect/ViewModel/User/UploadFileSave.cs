using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.User
{
    public class UploadFileSave
    {
        public int? AttachmentTypeId { get; set; }

        [Required(ErrorMessage = "Please select a file.")]
        public IFormFile File { get; set; }
    }
}
