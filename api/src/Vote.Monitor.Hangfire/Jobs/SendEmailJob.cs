using Job.Contracts.Jobs;
using Vote.Monitor.Core.Services.Mailing.Contracts;

namespace Vote.Monitor.Hangfire.Jobs;

public class SendEmailJob(IMailService mailService, ILogger<SendEmailJob> logger) : ISendEmailJob
{
    public async Task SendAsync(string to, string subject, string body)
    {
        try
        {
            await mailService.SendAsync(new MailRequest(to, subject, body), CancellationToken.None);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured when sending mail");
            throw;
        }
    }
}
