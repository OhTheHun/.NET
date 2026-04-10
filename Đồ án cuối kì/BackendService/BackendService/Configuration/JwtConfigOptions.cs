namespace BackendService.Configuration
{
    public class JwtConfigOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int TokenValidityMiliSeconds { get; set; } = 0;
    }
}
