using FluentValidation;

namespace ViewModel.User
{
    public class ConfirmAttachment
    {
        public int UserId { get; set; }
        public int AttachmentId { get; set; }
        public string DocumentType { get; set; }
    }
    public class ConfirmAttachmentValidator : AbstractValidator<ConfirmAttachment>
    {
        public ConfirmAttachmentValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull();
            RuleFor(x => x.AttachmentId).NotEmpty().NotNull();
        }
    }

    public class AttachmentsRequest
    {
        public int? RequestId { get; set; }
        public string File { get; set; }
        public string DocumentType { get; set; }
        public bool? LastAttachment { get; set; }

    }
}
