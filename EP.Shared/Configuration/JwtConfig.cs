namespace EP.Shared.Configuration
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public int AccessTokenExpiration { get; set; }
        public int RefreshTokenExpiration { get; set; }

        public const string Issuer = "https://localhost:44335/api/";
        public const string Audience = "https://localhost:44335/api/";
    }
}
