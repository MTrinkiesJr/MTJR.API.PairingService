using System.Text;
using Networking.Native;
using Newtonsoft.Json;

namespace MTJR.API.PairingService.Model.Messages
{
    public class EventMessage
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("args")] public object[] Args { get; set; } = new object[1];

        [JsonIgnore] 
        public object Message { get; set; }

        public EventMessage(EventMessageName name, object message, AesSecurityProvider provider)
        {
            Message = message;
            Name = name.ToString();
            //encrypt data with logged in pin and session id (wrapped in an AesSecurityProvider)
            Args[0] = provider.EncryptData(message);
        }

        public EventMessage()
        {
            
        }
    }
}
