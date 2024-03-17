namespace ViewModel.Attachment
{
    public class AttachmentFilter
    {
        public int? Id { get; set; }
        public string Guid { get; set; }
        public bool? WithData { get; set; }
        public int? AttachmentTypeId { get; set; }

        public List<int> AttachmentTypes { get; set; } = new List<int>() { };

        public string ModalId { get; set; }

        public int? UserId { get; set; }
        public int? RecordId { get; set; }

        public List<int> RecordIds { get; set; } = new List<int>() { };

        public bool? ShowHtmlResponse { get; set; }
        public bool? ShowThumbnail { get; set; }
    }
    public class StaticDownloadFilter
    {
        public string Name { get; set; }

    }
}
