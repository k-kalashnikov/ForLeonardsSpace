using DnsClient;
using MailKit.Net.Smtp;
using MailKit.Security;
using Masofa.Common.Models.Notifications;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Services.EmailSender;
using Masofa.DataAccess;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Masofa.BusinessLogic.Common.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly LookupClient _dns = new LookupClient();
        private SmtpOptions Opt { get; set; }
        private MasofaCommonDbContext CommonDbContext { get; set; }
        private string TemplatesPath { get; set; }

        public EmailSender(MasofaCommonDbContext commonDbContext, IOptions<SmtpOptions> options, string templatesPath)
        {
            Opt = options.Value;
            TemplatesPath = templatesPath;
            CommonDbContext = commonDbContext;
        }

        public async Task<bool> SendEmailAsync(List<string> to, string? subject, string? body, List<string>? cc)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(Opt.From));
                foreach (var recipient in to)
                {
                    message.To.Add(MailboxAddress.Parse(recipient));
                }
                message.Subject = subject;

                if(cc != null)
                {
                    foreach (var recipient in cc ?? [])
                    {
                        message.Cc.Add(MailboxAddress.Parse(recipient));
                    }
                }

                var newEmail = new EmailMessage
                {
                    Sender = Opt.From,
                    Recipients = to,
                    CarbonCopy = cc,
                    Body = body,
                    Subject = subject
                };

                await CommonDbContext.EmailMessages.AddAsync(newEmail);
                await CommonDbContext.SaveChangesAsync();

                var bodyBuilder = new BodyBuilder { HtmlBody = body };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                var socketOptions = Opt.UseSsl
                    ? SecureSocketOptions.SslOnConnect
                    : SecureSocketOptions.StartTlsWhenAvailable;

                await client.ConnectAsync(Opt.Host, Opt.Port, socketOptions);

                if (!string.IsNullOrEmpty(Opt.User) && !string.IsNullOrEmpty(Opt.Password))
                {
                    await client.AuthenticateAsync(Opt.User, Opt.Password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Send failed: {ex}");
                return false;
            }
        }

        public async Task<string> LoadTemplateAsync(string templateName, string culture = "en")
        {
            var filePath = Path.Combine(TemplatesPath, $"{templateName}.{culture}.html");

            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(TemplatesPath, $"{templateName}.en.html");
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Could not find template: {filePath}");
            }

            return await File.ReadAllTextAsync(filePath);
        }
    }
}
