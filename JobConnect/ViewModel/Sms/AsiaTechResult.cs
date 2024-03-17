namespace ViewModel.Sms
{
    public class AsiaTechResult
    {
        public string Message { get; set; }
        public bool Succeeded { get; set; }
        public List<string> Data { get; set; }
        public int ResultCode { get; set; }
    }
    public class AsiaTechResponse
    {
        public string SourceAddress { get; set; }
        public string DestinationAddress { get; set; }
        public string MessageText { get; set; }
    }
}
