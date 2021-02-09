using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyProject.Core.Services;
using MyProject.Core.ViewModels;
using MyProject.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
            private readonly TimeSpan _expiry;

            private static byte[] GenerateSecurityKey(int length)
            {
                var rtn = new byte[length];
                using var rng = new RNGCryptoServiceProvider();
                rng.GetBytes(rtn);
                return rtn;
            }

            public JwtGenerator(TimeSpan expiry)
            {
                _tokenSecurityKey = GenerateSecurityKey(64);
                _securityKey = new(_tokenSecurityKey);
                _credentials = new(_securityKey, _algorithmName);
                _expiry = expiry;
            }

            public string GenerateJwt(
                Claim[] claims, string issuer, string audience)
            {
                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now + _expiry,
                    signingCredentials: _credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
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
                        ValidAlgorithms = new[] { _algorithmName },
                    }, out SecurityToken token);

                JwtSecurityToken jwtToken = (JwtSecurityToken)token;
                return jwtToken.Claims;
            }
        }

        private const string _issuerKey = "JwtIssuer";
        private const string _audienceKey = "JwtAudience";

        private readonly static JwtGenerator refrechTokenGenerator = new(TimeSpan.FromDays(30));
        private readonly static JwtGenerator accessTokenGenerator = new(TimeSpan.FromMinutes(20));

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

        public UserIdentity ReadUserIdentity(string token, TokenType type)
        {
            return FromClaims(
                GetJwtGenerator(type).ParseJwt(
                    token, _config[_issuerKey], _config[_audienceKey]));
        }

        public string GenerateToken(UserIdentity identity, TokenType type)
        {
            return GetJwtGenerator(type).GenerateJwt(
                ToClaims(identity), _config[_issuerKey], _config[_audienceKey]);
        }

        public Claim[] ToClaims(UserIdentity identity)
        {
            return new Claim[]
            {
                new("id", identity.Id.ToString()),
                new("username", identity.Username),
                new("realname", identity.RealName),
                new("type", identity.Type.ToString())
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
            if (!Enum.TryParse<UserType>(GetValue("type"), out UserType type))
                return null;

            return new UserIdentity
            {
                Id = id,
                Username = username,
                RealName = realname,
                Type = type,
            };
        }

    }
}
