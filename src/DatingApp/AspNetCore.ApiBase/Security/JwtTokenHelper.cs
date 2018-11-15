using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AspNetCore.ApiBase.Security
{
    //https://www.carlrippon.com/asp-net-core-web-api-multi-tenant-jwts/
    public static class JwtTokenHelper
    {
        //Assymetric
        public static dynamic CreateJwtTokenSigningWithRsaSecurityKey(string userId, string userName, IEnumerable<string> roles, int minuteExpiry, RsaSecurityKey key, string issuer, string audience, params string[] scopes)
        {
            var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, userId),
                            new Claim(ClaimTypes.Name, userName),
                            new Claim(JwtRegisteredClaimNames.Sub, userId),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, userName)
                        };

            // add scopes
            foreach (var scope in scopes)
            {
                claims.Add(new Claim("scope", scope));
            }

            //Add roles
            foreach (string role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            return CreateJwtToken(minuteExpiry, issuer, audience, claims, creds);
        }

        //Assymetric
        public static dynamic CreateJwtTokenSigningWithCertificateSecurityKey(string userId, string userName, IEnumerable<string> roles, int minuteExpiry, X509SecurityKey key, string issuer, string audience, params string[] scopes)
        {
            var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, userId),
                            new Claim(ClaimTypes.Name, userName),
                            new Claim(JwtRegisteredClaimNames.Sub, userId),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, userName)
                        };

            // add scopes
            foreach (var scope in scopes)
            {
                claims.Add(new Claim("scope", scope));
            }

            //Add roles
            foreach (string role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            return CreateJwtToken(minuteExpiry, issuer, audience, claims, creds);
        }

        //Symmetric
        public static dynamic CreateJwtTokenSigningWithKey(string userId, string userName, IEnumerable<string> roles, int minuteExpiry, string hmacKey, string issuer, string audience, params string[] scopes)
        {
            var claims = new List<Claim>()
                        {
                            new Claim(ClaimTypes.NameIdentifier, userId),
                            new Claim(ClaimTypes.Name, userName),
                            new Claim(JwtRegisteredClaimNames.Sub, userId),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, userName)
                        };

            // add scopes
            foreach (var scope in scopes)
            {
                claims.Add(new Claim("scope", scope));
            }

            //Add roles
            foreach (string role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(hmacKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            return CreateJwtToken(minuteExpiry, issuer, audience, claims, creds);
        }

        private static dynamic CreateJwtToken(int minuteExpiry, string issuer, string audience, List<Claim> claims, SigningCredentials creds)
        {
            var token = new JwtSecurityToken(
                        issuer,
                        audience,
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(minuteExpiry),
                        signingCredentials: creds);

            var results = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };

            return results;
        }
    }
}
