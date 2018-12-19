using System;

namespace AspNetCore.ApiBase.Security
{
    public class JwtToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
