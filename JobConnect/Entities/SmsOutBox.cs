namespace Entities
{
    public class SmsOutBox
    {
        public int Id { get; set; }
        public string SmsNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }

        public string ErrorCode { get; set; }
        public string ProvideId { get; set; }
        public int? RecordId { get; set; }
        public int? SmsOutBoxTypeId { get; set; }
        public string DisplayName { get; set; }
        public string NationalCode { get; set; }

        public DateTime DateTime { get; set; }

    }
}
