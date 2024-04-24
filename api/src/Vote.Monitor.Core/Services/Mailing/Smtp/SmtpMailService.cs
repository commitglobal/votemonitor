using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Vote.Monitor.Core.Services.Mailing.Contracts;

namespace Vote.Monitor.Core.Services.Mailing.Smtp;
public class SmtpMailService(IOptions<SmtpOptions> settings, ILogger<SmtpMailService> logger)
    : IMailService
{
    private readonly SmtpOptions _options = settings.Value;

    public async Task SendAsync(MailRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var email = new MimeMessage();

            // From
            email.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));

            // To
            email.To.Add(MailboxAddress.Parse(request.To));

            // Content
            var builder = new BodyBuilder();
            email.Sender = new MailboxAddress(_options.SenderName, _options.SenderEmail);
            email.Subject = request.Subject;
            builder.HtmlBody = request.Body;

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTlsWhenAvailable, cancellationToken);
            await smtp.AuthenticateAsync(_options.UserName, _options.Password, cancellationToken);
            await smtp.SendAsync(email, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
