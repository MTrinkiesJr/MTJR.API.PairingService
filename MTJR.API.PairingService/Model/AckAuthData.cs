using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Pairing;

namespace MTJR.API.PairingService.Model
{
    public class AckAuthData 
    {
        [JsonProperty("auth_type")]
        public string AuthType { get; set; }
        [JsonProperty("request_id")]
        public string RequestId { get; set; }
        [JsonProperty("ServerAckMsg")]
        public string Ack { get; set; }

        public AckAuthData(string authType, string pin, string data, SPCApiBridge spiApi)
        {
            var regexPattern = "[{\\\"\\w:]GeneratorClientHello[\\\\\\\":]*([\\d\\w]*)";
            var requestData = Regex.Match(data, regexPattern).Groups[1].Value;

            var regexRequestPattern = "[{\\\"\\w:]request_id[\\\\\\\":]*([\\d\\w]*)";
            var requestId = Regex.Match(data, regexRequestPattern).Groups[1].Value;
            AuthType = authType;
            
            var parsed = spiApi.ParseClientHello(pin, requestData);

            if (parsed)
            {
                RequestId = requestId;
                Ack = spiApi.GenerateServerAck();
            }
        }
    }
}
