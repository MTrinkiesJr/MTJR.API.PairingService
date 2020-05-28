using Newtonsoft.Json;

namespace MTJR.API.PairingService.Model.Messages
{
    public class ReceivedMessage
    {
        [JsonProperty("plugin")]
        public string Plugin { get; set; }
        [JsonProperty("api")]
        public string Api { get; set; }
        [JsonProperty("result")]
        public object Result { get; set; }
    }
}
