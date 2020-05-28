using Newtonsoft.Json;

namespace MTJR.API.PairingService.Extensions
{
    public static class ObjectExtensions
    {
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
