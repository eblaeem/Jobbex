namespace ViewModel.Sms
{
    public class SmsOutBoxResponse:IDataTable
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string NationalCode { get; set; }
        public string DisplayName { get; set; }
        public string MobileNumber { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public DateTime DateTime { get; set; }
        public string DateTimeString { get; set; }
        public int TotalRowCount { get; set; }
        public string ProviderId { get; set; }
        public int? SmsOutBoxTypeId { get; set; }
        public string SmsOutBoxTypeName { get; set; }

        public List<Column> GetColumns()
        {
            return new List<Column>()
            {
                new Column(nameof(DateTimeString),"تاریخ"),
                new Column(nameof(DisplayName),"نام،نام خانوادگی"),
                new Column(nameof(MobileNumber),"شماره همراه"),
                new Column(nameof(NationalCode),"کد ملی"),
                new Column(nameof(UserName),"نام کاربری "),
                new Column(nameof(ProviderId),"شناسه ثبت"),
                new Column(nameof(SmsOutBoxTypeName),"نوع"),
                new Column(nameof(Message),"متن"),
            };
        }
    }
}
