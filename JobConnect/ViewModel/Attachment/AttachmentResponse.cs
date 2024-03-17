namespace ViewModel.Attachment
{
    public class Attachments
    {
        public string Data { get; set; }
        public string Columns { get; set; }
        public string ModalId { get; set; }
    }
    public class AttachmentFileType
    {
        public bool IsImage { get; set; }
        public bool IsPdf { get; set; }
        public bool IsVideo { get; set; }
        public bool IsAudio { get; set; }
        public bool OtherFiles { get; set; }
    }

    public class AttachmentResponse : IDataTable
    {
        public AttachmentResponse()
        {

        }
        public AttachmentResponse(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
        public int Id { get; set; }
        public string FileName { get; set; }

        public string DisplayName { get; set; }
        public string NationalCode { get; set; }
        public int? UserId { get; set; }
        public string FileType { get; set; }

        public int FileSize { get; set; }
        public string FileData { get; set; }

        public int? AttachmentTypeId { get; set; }
        public string AttachmentTypeName { get; set; }
        public string AttachmentTypeEnglishName { get; set; }

        public string MimeType { get; set; }

        public bool IsImage { get; set; }
        public bool IsPdf { get; set; }
        public bool IsVideo { get; set; }
        public bool IsAudio { get; set; }


        public int TotalRowCount { get; set; }
        public int? RecordId { get; set; }

        public byte[] FileDataByte { get; set; }

        public string Guid { get; set; }

        public bool IsRegisterableToIme { get; set; }


        public bool? Confirm { get; set; }
        public string ConfirmDateTimeString { get; set; }
        public DateTime? ConfirmDateTime { get; set; }

        public bool? Reject { get; set; }
        public string RejectDateTimeString { get; set; }
        public DateTime? RejectDateTime { get; set; }

        public bool IsValid { get; set; }
        public string Message { get; set; }

        public string HtmlResponse { get; set; }

        public List<Column> GetColumns()
        {
            return new List<Column>()
            {
                new Column(nameof(FileName),"نام"){ IsSorting=false},
                new Column(nameof(FileType),"نوع"){ IsSorting=false},
                new Column(nameof(FileSize),"اندازه"){ IsSorting=false},
                new Column(nameof(AttachmentTypeName),"نوع ضمیمه"){ IsSorting=false}
            };
        }
    }
}
