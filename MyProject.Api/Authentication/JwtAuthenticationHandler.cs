using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MyProject.Core.Services;
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

        private AuthenticateResult HandleAuthenticate()
        {
            try
            {
                var authorization = Request.Headers[HeaderNames.Authorization].ToString();
                if (!authorization.StartsWith("Bearer "))
                    return AuthenticateResult.Fail("No user identity given");
                authorization = authorization[7..];

                var identity = _identityService.ReadUserIdentity(authorization, TokenType.AccessToken);
                if (identity is null)
                    return AuthenticateResult.Fail("No user identity given");

                Request.HttpContext.Items[IdentityKey] = identity;

                Claim[] claims = _identityService.ToClaims(identity);
                ClaimsIdentity claimsIdentity = new(claims, Scheme);
                ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
                AuthenticationTicket ticket = new(claimsPrincipal, Scheme);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("No user identity given");
            }
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var result = HandleAuthenticate();
            if (!result.Succeeded)
            {
                if (Context.GetEndpoint()?.Metadata?.GetMetadata<IAllowAnonymous>() is not null)
                    result = AuthenticateResult.NoResult();
            }
            return Task.FromResult(result);
        }
    }
}
