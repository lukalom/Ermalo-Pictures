using EP.Shared.Configuration;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EP.Shared
{
    public class EmailSender : IEmailSender
    {
        public SendGridConfig SendGridConfig { get; set; }
        public EmailSender(IOptionsMonitor<SendGridConfig> optionsMonitor)
        {
            SendGridConfig = optionsMonitor.CurrentValue;
        }

        public Task SendEmailAsync(string userEmail, string subject, string htmlMessage)
        {
            var client = new SendGridClient(SendGridConfig.SecretKey);
            var from = new EmailAddress(SendGridConfig.MyEmail, SendGridConfig.CompanyName);
            var to = new EmailAddress(userEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

            return client.SendEmailAsync(msg);
        }
    }
}
