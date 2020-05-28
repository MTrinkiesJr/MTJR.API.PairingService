using Newtonsoft.Json;

namespace MTJR.API.PairingService.Model.Messages
{
    public class ButtonMessage : IMessage
    {
        [JsonProperty("method")]
        public string Method => "POST";

        [JsonProperty("body")]
        public ButtonBody  ButtonBody { get; set; }

        public ButtonMessage(string keyCode, string duid)
        {
            ButtonBody = new ButtonBody(keyCode, duid);
        }
    }
}
