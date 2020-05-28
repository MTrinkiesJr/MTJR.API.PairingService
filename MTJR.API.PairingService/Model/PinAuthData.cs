using Newtonsoft.Json;
using Pairing;

namespace MTJR.API.PairingService.Model
{
    public class PinAuthData
    {
        [JsonProperty("auth_type")]
        public string AuthType { get; set; }
        [JsonProperty("GeneratorServerHello")]
        public string Pin { get; set; }

        public PinAuthData(string authType, string pin, SPCApiBridge spcApi)
        {
            AuthType = authType;
            string serverHello = spcApi.GenerateServerHello(pin);
            Pin = serverHello;
        }
    }
}
