using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTJR.API.PairingService.Model
{
    public class ApiCallResponse
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ApiCallMethod Method { get; set; }
        public string Url { get; set; }
        public string  Data { get; set; }

        public ApiCallResponse(ApiCallMethod method, string url, string data = null)
        {
            Method = method;
            Url = url;
            Data = data.Replace("\"", "");
        }
    }
}
