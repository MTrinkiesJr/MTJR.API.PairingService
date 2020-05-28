using Newtonsoft.Json;

namespace MTJR.API.PairingService.Model.Messages
{
    public class GetDuidBody : IMessage
    {
        [JsonProperty("plugin")]
        public string Plugin => "NNavi";

        [JsonProperty("api")]
        public string Api => "GetDUID";
        [JsonProperty("version")]
        public string Version => "1.000";
    }
}
