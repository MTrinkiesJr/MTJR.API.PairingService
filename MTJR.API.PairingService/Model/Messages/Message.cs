using Newtonsoft.Json;

namespace MTJR.API.PairingService.Model.Messages
{
    public class Message : IMessage
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }
        [JsonProperty("plugin")]
        public string Plugin { get; set; }
    }
}
