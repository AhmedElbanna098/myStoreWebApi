/*using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;

namespace MyStoreWebAPI.Services
{
    public class EmailSender
    {
        private readonly string apiKey;
        private readonly string fromEmail;
        private readonly string senderName;

        public EmailSender(IConfiguration configuration)
        {
            apiKey = configuration["EmailSender:APIkey"]!;
            fromEmail = configuration["EmailSender:FromEmail"]!;
            senderName = configuration["EmailSender:SenderName"]!;
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey), "API key cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(fromEmail))
            {
                throw new ArgumentNullException(nameof(fromEmail), "From email cannot be null or empty.");
            }
            if (string.IsNullOrEmpty(senderName))
            {
                throw new ArgumentNullException(nameof(senderName), "Sender name cannot be null or empty.");
            }
        }
        public async Task SendEmail(string subject, string toEmail, string username, string message)
        {
            
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, senderName);
            var to = new EmailAddress(toEmail, username);
            var plainTextContent = message;
            var htmlContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
*/