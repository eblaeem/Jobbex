namespace ViewModel.Audit
{
    public class AuditResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PersonName { get; set; }
        public string NationalCode { get; set; }
        public DateTime? DateTime { get; set; }
        public string DateString { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string TypeName { get; set; }

        public string IpAddress { get; set; }
        public int TotalRowCount { get; set; }

    }
}
