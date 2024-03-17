namespace ViewModel
{
    public class ResponseBase
    {
        public ResponseBase()
        {
        }
        public ResponseBase(bool isValid, string message = "")
        {
            IsValid = isValid;
            Message = message;
        }
        public int? Id { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }
    }
}
