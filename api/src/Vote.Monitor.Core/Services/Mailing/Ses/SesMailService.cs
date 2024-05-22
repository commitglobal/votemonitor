using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Core.Services.Mailing.Contracts;

namespace Vote.Monitor.Core.Services.Mailing.Ses;

public class SesMailService(IOptions<SesOptions> settings,
    IAmazonSimpleEmailService sesMailService,
    ILogger<SesMailService> logger)
    : IMailService
{
    private readonly SesOptions _options = settings.Value;

    public async Task SendAsync(MailRequest mailRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            var mailBody = new Body
            {
                Html = new Content(mailRequest.Body)
            };
            var message = new Message(new Content(mailRequest.Subject), mailBody);
            var destination = new Destination([mailRequest.To]);
            var request = new SendEmailRequest(_options.SenderEmail, destination, message);
            await sesMailService.SendEmailAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
