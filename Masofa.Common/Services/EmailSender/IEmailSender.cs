using Masofa.Common.Models.Identity;

namespace Masofa.Common.Services.EmailSender
{
    public interface IEmailSender
    {
        Task<bool> SendEmailAsync(List<string> to, string subject, string body, List<string>? cc);
        Task<string> LoadTemplateAsync(string templateName, string culture = "en");
    }
}
