using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ViewModel.Attachment
{
    public class AttachmentSave
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        [Display(Name = "نوع ضمیمه")]
        public int? AttachmentTypeId { get; set; }
        public string AttachmentTypeName { get; set; }
        public string FileName { get; set; }

        [Display(Name = "فایل")]
        public string FileData { get; set; }
        public string FileType { get; set; }
        public int FileSize { get; set; }

        public int? RecordId { get; set; }
        public List<LabelValue> AttachmentTypes { get; set; }

        public byte[] FileDataBytes { get; set; }

        public IFormFile FormFile { get; set; }
        public bool ImageOrPdf { get; set; }
    }
    public class AttachmentSaveValidator : AbstractValidator<AttachmentSave>
    {
        public AttachmentSaveValidator()
        {
            RuleFor(x => x.FileData).NotEmpty().NotNull();
            RuleFor(x => x.AttachmentTypeId).NotEmpty().NotNull();
        }
    }
}
