using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace COVIDSafe.Watch.DummyBackend
{
    public static class JsonExtensions
    {
        public static T ReadAsJson<T>(this HttpRequest request)
        {
            using var sr = new StreamReader(request.Body);
            using var jtr = new JsonTextReader(sr);
            var serializer = new JsonSerializer();

            return serializer.Deserialize<T>(jtr);
        }

        public static string CreateWebToken(string phoneNumber)
        {
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Audience = "COVIDsafe",
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("sub", phoneNumber),
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddYears(1),
                NotBefore = null,
            };

            // Unsigned and unencrypted because this is a dummy system
            var jwt =  handler.CreateEncodedJwt(descriptor);
            return jwt;
        }

        public static string GetJwtSubject(this HttpRequest request)
        {
            var authorizationHeaderValue = request.Headers[HeaderNames.Authorization].ToString();
            if (!authorizationHeaderValue.StartsWith("Bearer "))
            {
                return string.Empty;
            }

            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(authorizationHeaderValue[7..]);
            return jwt.Subject;
        }
    }
}
