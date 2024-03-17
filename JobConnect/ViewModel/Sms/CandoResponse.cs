using Newtonsoft.Json;

namespace ViewModel.Sms
{
    public class CandoResponse
    {
        [JsonProperty("sender")]
        public string Sender { get; set; }
        [JsonProperty("recipient")]
        public string Recipient { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("customerId")]
        public long? CustomerId { get; set; }

    }
    public class CandoResult
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public List<CandoResultData> Data { get; set; } = new List<CandoResultData>();

    }
    public class CandoResultData
    {
        public long? ServerId { get; set; }
        public long? CustomerId { get; set; }
        public string ErrorCode { get; set; }
    }
}
