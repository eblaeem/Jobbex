namespace ViewModel
{
    public class LogModel
    {
        public string Message { get; set; }
        public string AccessToken { get; set; }
        public string StackTrace { get; set; }
        public string MethodeName { get; set; }
        public string ApplicationName { get; set; }
        public DateTime? EventDateTime { get; set; }
        public bool IsError { get; set; }
        public string IpAddress { get; set; }
        public string Color { get; set; }
    }
}
