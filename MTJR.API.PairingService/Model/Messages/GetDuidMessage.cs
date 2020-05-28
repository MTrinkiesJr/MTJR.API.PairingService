using Newtonsoft.Json;

namespace MTJR.API.PairingService.Model.Messages
{
    public class GetDuidMessage : IMessage
    {
        [JsonProperty("method")]
        public string Method => "POST";
        [JsonProperty("body")]
        public GetDuidBody GetDuidBody => new GetDuidBody();
    }
}
