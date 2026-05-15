namespace BackendService.Configuration
{
    public class ConfigOptions
    {
        public const string Config = "Config";
        public List<string> AllowedOrigins { get; set; } = [];
        public JwtConfigOptions JwtConfig { get; set; } = new JwtConfigOptions();
        public EmailOptions EmailOptions { get; set; } = new EmailOptions();
    }

    public class EmailOptions
    {
        public EmailSender Sender { get; set; } = new EmailSender();
        public EmailCredential Credential { get; set; } = new EmailCredential();
    }

    public class EmailSender
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class EmailCredential
    {
        public string SmtpServer { get; set; } = string.Empty;
        public string Port { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}