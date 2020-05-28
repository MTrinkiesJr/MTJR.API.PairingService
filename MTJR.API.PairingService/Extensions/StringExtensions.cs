using Newtonsoft.Json;

namespace MTJR.API.PairingService.Extensions
{
    public static class StringExtensions
    {
        public static T DeserializeTo<T>(this string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
