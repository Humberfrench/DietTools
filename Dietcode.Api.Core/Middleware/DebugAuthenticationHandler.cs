using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;


namespace Dietcode.Api.Core.Middleware
{
    public class DebugAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                            ILoggerFactory logger, UrlEncoder encoder) :
                 AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Claims fakes para debug
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, "DebugUser"),
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.GroupSid, "28"),
            new Claim(ClaimTypes.Role, "Admin")
        };

            var identity = new ClaimsIdentity(claims, "DebugAuth");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "DebugAuth");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
