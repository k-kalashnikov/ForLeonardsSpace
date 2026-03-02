namespace Masofa.Common.Models.SystemCrical
{
    public class SmtpOptions
    {
        public string CallbackUrl { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string? User { get; set; }
        public string? Password { get; set; }
        public string From { get; set; } = string.Empty;
        public bool UseSsl { get; set; }
        public string? HealthCheckRecipient { get; set; }
        public string HealthCheckLanguage { get; set; } = "ru";
    }
}
