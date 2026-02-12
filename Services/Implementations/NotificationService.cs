using CRM_Backend.Exceptions;
using CRM_Backend.Security.Email;
using CRM_Backend.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CRM_Backend.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly EmailSettings _email;
        private readonly SmtpSettings _smtp;

        public NotificationService(
            IOptions<EmailSettings> emailOptions,
            IOptions<SmtpSettings> smtpOptions)
        {
            _email = emailOptions?.Value
                ?? throw new ArgumentNullException(nameof(emailOptions));

            _smtp = smtpOptions?.Value
                ?? throw new ArgumentNullException(nameof(smtpOptions));
        }

        public async Task SendPasswordResetAsync(
            string userEmail,
            string resetLink)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
                throw new ValidationException("User email is required.");

            if (string.IsNullOrWhiteSpace(resetLink))
                throw new ValidationException("Reset link is required.");

            var to = string.IsNullOrWhiteSpace(_email.OverrideRecipient)
                ? userEmail
                : _email.OverrideRecipient;

            try
            {
                using var message = new MailMessage
                {
                    From = new MailAddress(_smtp.From),
                    Subject = "Reset your CRM password",
                    Body =
                        $"Hello,\n\n" +
                        $"Click the link below to reset your password:\n\n" +
                        $"{resetLink}\n\n" +
                        $"If you did not request this, ignore this email.",
                    IsBodyHtml = false
                };

                message.To.Add(to);

                using var client = new SmtpClient(_smtp.Host, _smtp.Port)
                {
                    Credentials = new NetworkCredential(
                        _smtp.Username,
                        _smtp.Password
                    ),
                    EnableSsl = _smtp.EnableSsl
                };

                await client.SendMailAsync(message);
            }
            catch (SmtpException ex)
            {
                throw new InternalServerException(
                    $"Email sending failed: {ex.Message}");
            }
        }
    }
}
