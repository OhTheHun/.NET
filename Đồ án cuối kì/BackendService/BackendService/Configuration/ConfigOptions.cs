namespace BackendService.Configuration
{
    public class ConfigOptions
    {
        public const string Config = "Config";
        public List<string> AllowedOrigins { get; set; } = [];
        public JwtConfigOptions JwtConfig { get; set; } = new JwtConfigOptions();
    }
}