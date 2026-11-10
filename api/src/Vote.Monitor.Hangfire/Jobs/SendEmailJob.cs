using Job.Contracts.Jobs;
using Vote.Monitor.Core.Services.Mailing.Contracts;

namespace Vote.Monitor.Hangfire.Jobs;

public class SendEmailJob(IMailService mailService, ILogger<SendEmailJob> logger) : ISendEmailJob
{
    public Task SendAsync(string to, string subject, string body) => Send(new MailRequest(to, subject, body));

    public Task SendAsync(string to, string subject, string body, string textBody) =>
        Send(new MailRequest(to, subject, body, textBody));

    private async Task Send(MailRequest request)
    {
        try
        {
            await mailService.SendAsync(request, CancellationToken.None);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured when sending mail");
            throw;
        }
    }
}
