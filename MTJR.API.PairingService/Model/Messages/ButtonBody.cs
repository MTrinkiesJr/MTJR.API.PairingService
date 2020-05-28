using Newtonsoft.Json;

namespace MTJR.API.PairingService.Model.Messages
{
    public class ButtonBody : IMessage
    {
        [JsonProperty("plugin")]
        public string Plugin => "RemoteControl";
        [JsonProperty("version")]
        public string Version => "1.000";
        [JsonProperty("api")]
        public string Api => "SendRemoteKey";

        [JsonProperty("param1")]
        public string Duid { get; }

        [JsonProperty("param2")] 
        public string Method => "Click";
        [JsonProperty("param3")]
        public string KeyCode { get; set; }
        [JsonProperty("param4")] public string Something => "false";

        public ButtonBody( string keyCode, string duid)
        {
            Duid = duid;
            KeyCode = keyCode;
        }
    }
}
