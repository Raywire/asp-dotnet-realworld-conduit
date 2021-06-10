using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Conduit.Services
{
    public class CloudMailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public CloudMailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Send(string subject, string message, string name, string email)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var mailFrom = _configuration["MailSettings:MailFrom"] ?? Environment.GetEnvironmentVariable("MAIL_FROM");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(mailFrom, "Conduit");
            var to = new EmailAddress(email, name);
            var htmlContent = "";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}
