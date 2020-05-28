using System;
using Microsoft.AspNetCore.Authentication;

namespace MTJR.API.PairingService.Authentication
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddGuidAuthentication(this AuthenticationBuilder builder, Action<GuidAuthenticationOptions> options)
        {
            return builder.AddScheme<GuidAuthenticationOptions, GuidAuthenticationHandler>(GuidAuthenticationOptions.DefaultScheme, options);
        }
    }
}
