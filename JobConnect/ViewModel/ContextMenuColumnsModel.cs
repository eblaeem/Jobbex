using Newtonsoft.Json;

namespace ViewModel
{
    public class ContextMenuColumnsModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
