namespace ViewModel.User
{
    public class UserResponse : IDataTable
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string NationalCode { get; set; }
        public string DateString { get; set; }
        public bool? HasCustomer { get; set; }
        public int TotalRowCount { get; set; }
        public DateTimeOffset? DateTime { get; set; }
        public string CustomerRequestId { get; set; }
        public string CustomerRequestState { get; set; }
        public bool IsSpotMarket { get; set; }
        public int? SpotCode { get; set; }
        public bool IsDerivativesMarket { get; set; }

        public int? SignatureAttachmentId { get; set; }
        public bool? HasSignature { get; set; }
        public bool? UserHasBankAccount { get; set; }
        public bool? IsSendAttachment { get; set; }
        public string ImeCreatedUserSms { get; set; }
        public string IntroducerName { get; set; }
        public int? IntroducerUserId { get; set; }
        public int? SubmitBrokerRequestId { get; set; }
        public List<Column> GetColumns()
        {
            return new List<Column>()
            {
                new Column(nameof(DateString),"تاریخ ثبت"),
                new Column(nameof(Username),"نام کاربری"),
                new Column(nameof(FirstName),"نام "),
                new Column(nameof(LastName),"نام خانوادگی"),
                new Column(nameof(NationalCode),"کد ملی"),
                new Column(nameof(PhoneNumber),"شماره همراه"),
                new Column(nameof(IntroducerName),"نام معرف"),
                new Column(nameof(HasCustomer),"مشتری کالا"),
                new Column(nameof(UserHasBankAccount),"معرفی حساب"),
                new Column(nameof(IsSpotMarket),"بازار فیزیکی"),
                new Column(nameof(IsDerivativesMarket),"بازار مشتقه"),
                new Column(nameof(CustomerRequestId),"شناسه ثبت کالا"),
                new Column(nameof(IsSendAttachment),"ثبت ضمیمه"),
                new Column(nameof(CustomerRequestState),"وضعیت ثبت درخواست"),
                new Column(nameof(ImeCreatedUserSms),"پیامک ارسالی"),
            };
        }
    }
}
