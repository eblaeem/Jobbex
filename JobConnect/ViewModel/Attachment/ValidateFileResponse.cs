namespace ViewModel.Attachment
{
    public class ValidateFileResponse
    {
        public ValidateFileResponse()
        {
        }
        public ValidateFileResponse(bool isValid, string message = "")
        {
            IsValid = isValid;
            Message = message;
        }
        public int? Id { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }

        public byte[] Bytes { get; set; }
    }
}
