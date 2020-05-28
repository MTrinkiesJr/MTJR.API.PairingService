using Microsoft.AspNetCore.Authentication;

namespace MTJR.API.PairingService.Authentication
{
    public class GuidAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "GUID";
        public string Scheme = DefaultScheme;
        public string AuthenticationType = DefaultScheme;
        public string Guid { get; set; }
    }
}
