namespace Entities
{
    public class Attachment
    {
        public int Id { get; set; }
        public int? AttachmentTypeId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public byte[] FileData { get; set; }
        public int FileSize { get; set; }
        public int? UserId { get; set; }
        public int? RecordId { get; set; }
        public DateTime InsertDateTime { get; set; }
        public Guid? Guid { get; set; }
        public string? TableName { get; set; }

        public bool? Confirm { get; set; }
        public DateTime? ConfirmDateTime { get; set; }

        public bool? Reject { get; set; }
        public DateTime? RejectDateTime { get; set; }
    }
}
