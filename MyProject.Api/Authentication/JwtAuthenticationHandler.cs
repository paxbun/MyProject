using MyProject.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MyProject.Api.Authentication
{
    public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public new const string Scheme = "JwtAuthenticationHandlerScheme";

        public const string IdentityKey = "JwtAuthenticationHandlerIdentity";

        private readonly IUserIdentityService _identityService;

        public JwtAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IUserIdentityService identityService)
            : base(options, logger, encoder, clock)
        {
            _identityService = identityService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var endpoint = Context.GetEndpoint();

            try
            {
                var authorization = Request.Headers[HeaderNames.Authorization].ToString();
                if (!authorization.StartsWith("Bearer "))
                    return Task.FromResult(AuthenticateResult.Fail("No user identity given"));
                authorization = authorization[7..];

                var identity = _identityService.ReadUserIdentity(authorization, TokenType.AccessToken);
                if (identity == null)
                    return Task.FromResult(AuthenticateResult.Fail("No user identity given"));

                Request.HttpContext.Items[IdentityKey] = identity;

                Claim[] claims = _identityService.ToClaims(identity);
                ClaimsIdentity claimsIdentity = new(claims, Scheme);
                ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
                AuthenticationTicket ticket = new(claimsPrincipal, Scheme);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                    return Task.FromResult(AuthenticateResult.NoResult());

                return Task.FromResult(AuthenticateResult.Fail(ex));
            }
        }
    }
}
