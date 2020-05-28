using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MTJR.API.PairingService.Authentication
{
    public class GuidAuthenticationHandler : AuthenticationHandler<GuidAuthenticationOptions>
    {
        public GuidAuthenticationHandler(IOptionsMonitor<GuidAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("X-Api-Guid", out var headerValues))
            {
                return AuthenticateResult.NoResult();
            }

            var guid = headerValues.FirstOrDefault();

            if (!headerValues.Any() || string.IsNullOrEmpty(guid))
            {
                return AuthenticateResult.NoResult();
            }

            if (Options.Guid.Equals(guid, StringComparison.InvariantCultureIgnoreCase))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Owner")
                };

                var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
                var identities = new List<ClaimsIdentity> { identity };
                var principal = new ClaimsPrincipal(identities);
                var ticket = new AuthenticationTicket(principal, Options.Scheme);

                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Authentication failed");
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            return base.HandleChallengeAsync(properties);
        }
    }
}
