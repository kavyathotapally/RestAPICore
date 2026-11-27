
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestAPICore.Auth;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace RestAPICore.Authentication
{
    public class CustomAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult>
            HandleAuthenticateAsync()
        {
            string token = Request.Headers["access-token"].ToString();

            // If access-token header is absent, check Bearer token
            if (string.IsNullOrEmpty(token))
            {
                string authorization =
                    Request.Headers["Authorization"].ToString();

                if (authorization.StartsWith(
                    "Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring(7).Trim();
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                return Task.FromResult(
                    AuthenticateResult.NoResult());
            }

            try
            {
                // Validate using your existing AccessToken class
                AuthPrincipal principal = new AuthPrincipal(token);

                // Store your custom principal for existing application code
                Context.Items["AuthPrincipal"] = principal;

                var claims = new[]
                {
                    new Claim(
                        ClaimTypes.Name,
                        principal.Identity.Name ?? string.Empty)
                };

                var identity = new ClaimsIdentity(
                    claims,
                    Scheme.Name);

                var claimsPrincipal =
                    new ClaimsPrincipal(identity);

                var ticket = new AuthenticationTicket(
                    claimsPrincipal,
                    Scheme.Name);

                return Task.FromResult(
                    AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    AuthenticateResult.Fail(ex.Message));
            }
        }
    }
}
