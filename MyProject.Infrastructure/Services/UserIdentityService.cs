using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyProject.Core.Services;
using MyProject.Core.ViewModels;
using MyProject.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;

namespace MyProject.Infrastructure.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private class JwtGenerator
        {
            private const string _algorithmName = SecurityAlgorithms.HmacSha256;
            private readonly byte[] _tokenSecurityKey;
            private readonly SymmetricSecurityKey _securityKey;
            private readonly SigningCredentials _credentials;

            private static byte[] GenerateSecurityKey(int length)
            {
                var rtn = new byte[length];
                using var rng = new RNGCryptoServiceProvider();
                rng.GetBytes(rtn);
                return rtn;
            }

            public JwtGenerator()
            {
                _tokenSecurityKey = GenerateSecurityKey(64);
                _securityKey = new(_tokenSecurityKey);
                _credentials = new(_securityKey, _algorithmName);
            }

            public string GenerateJwt(
                Claim[] claims, string issuer, string audience, TimeSpan expiry)
            {
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow + expiry,
                    signingCredentials: _credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }

            // https://stackoverflow.com/questions/39728519/jwtsecuritytoken-doesnt-expire-when-it-should
            private bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken token, TokenValidationParameters @params)
            {
                if (expires != null)
                {
                    return expires > DateTime.UtcNow;
                }
                return false;
            }

            public IEnumerable<Claim> ParseJwt(
                string tokenString, string issuer, string audience)
            {
                new JwtSecurityTokenHandler().ValidateToken(
                    tokenString,
                    new TokenValidationParameters
                    {
                        IssuerSigningKey = _securityKey,
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        RequireExpirationTime = true,
                        LifetimeValidator = LifetimeValidator,
                        ValidAlgorithms = new[] { _algorithmName },
                    }, out SecurityToken token);

                JwtSecurityToken jwtToken = (JwtSecurityToken)token;
                return jwtToken.Claims;
            }
        }

        private const string _issuerKey = "JwtIssuer";
        private const string _audienceKey = "JwtAudience";

        private readonly static JwtGenerator refrechTokenGenerator = new();
        private readonly static JwtGenerator accessTokenGenerator = new();

        private readonly IConfiguration _config;

        private static JwtGenerator GetJwtGenerator(TokenType type)
        {
            return type switch
            {
                TokenType.RefreshToken => refrechTokenGenerator,
                TokenType.AccessToken => accessTokenGenerator,
                _ => null
            };
        }

        public UserIdentityService(IConfiguration config)
        {
            _config = config;
        }

        public UserIdentity ReadUserIdentity(string token, TokenType type, IPAddress ip)
        {
            IEnumerable<Claim> claims;
            try
            {
                claims = GetJwtGenerator(type).ParseJwt(
                        token, _config[_issuerKey], _config[_audienceKey]);
            }
            catch
            {
                throw new UserIdentityServiceException();
            }

            var identity = FromClaims(claims);
            if (identity is null)
                throw new UserIdentityServiceException();

            if (type == TokenType.AccessToken && !identity.Ip.Equals(ip))
                throw new UserIdentityServiceException();

            return identity;
        }

        public string GenerateToken(UserIdentity identity, TokenType type)
        {
            TimeSpan expiry = (identity.Type, type) switch
            {
                (UserType.General, TokenType.AccessToken) => TimeSpan.FromMinutes(20),
                (UserType.General, TokenType.RefreshToken) => TimeSpan.FromDays(30),
                (UserType.Administrator, TokenType.AccessToken) => TimeSpan.FromMinutes(5),
                (UserType.Administrator, TokenType.RefreshToken) => TimeSpan.FromMinutes(20),
                _ => TimeSpan.Zero
            };

            return GetJwtGenerator(type).GenerateJwt(
                ToClaims(identity), _config[_issuerKey], _config[_audienceKey], expiry);
        }

        public Claim[] ToClaims(UserIdentity identity)
        {
            return new Claim[]
            {
                new("id", identity.Id.ToString()),
                new("username", identity.Username),
                new("realname", identity.RealName),
                new("type", identity.Type.ToString()),
                new("ip", identity.Ip.ToString())
            };
        }

        public UserIdentity FromClaims(IEnumerable<Claim> claims)
        {
            string GetValue(string type)
            {
                return (from c in claims
                        where c.Type == type
                        select c.Value).FirstOrDefault();
            }

            var id = int.Parse(GetValue("id"));
            var username = GetValue("username");
            var realname = GetValue("realname");
            if (!Enum.TryParse(GetValue("type"), out UserType type))
                return null;
            if (!IPAddress.TryParse(GetValue("ip"), out IPAddress ip))
                return null;

            return new UserIdentity
            {
                Id = id,
                Username = username,
                RealName = realname,
                Type = type,
                Ip = ip
            };
        }

    }
}
