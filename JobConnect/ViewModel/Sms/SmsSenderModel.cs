namespace ViewModel.Sms
{
    public class SmsSenderModel
    {
        public string SmsNumber { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public string MobileNumber { get; set; }
        public long? RecordId { get; set; }
        public int? SmsOutBoxTypeId { get; set; }

        public int? UserId { get; set; }
        public int? CustomerId { get; set; }

        public string ErrorCode { get; set; }
        public string DisplayName { get; set; }
        public string NationalCode { get; set; }
    }
    public class SmsSenderValidate
    {
        public int RecordId { get; set; }
        public string Token { get; set; }
    }
    public class SmsSenderResponse
    {
        public string Token { get; set; }
    }
}
