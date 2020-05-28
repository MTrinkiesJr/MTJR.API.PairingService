using Newtonsoft.Json;

namespace MTJR.API.PairingService.Model
{
    public class AuthRequest<T>
    {
        [JsonProperty("auth_data")]
        public T AuthData { get; set; }

        public AuthRequest()
        {
            
        }

        public AuthRequest(T authData)
        {
            AuthData = authData;
        }
    }
}
