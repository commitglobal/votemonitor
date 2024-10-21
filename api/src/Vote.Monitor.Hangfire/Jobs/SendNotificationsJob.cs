using Job.Contracts.Jobs;
using Vote.Monitor.Module.Notifications.Contracts;

namespace Vote.Monitor.Hangfire.Jobs;

public class SendNotificationJob(IPushNotificationService pushNotificationService, ILogger<SendNotificationJob> logger)
    : ISendNotificationJob
{
    public async Task SendAsync(List<string> userIdentifiers, string title, string body, CancellationToken ct = default)
    {
        try
        {
            await pushNotificationService.SendNotificationAsync(userIdentifiers, title, body, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured when sending mail");
            throw;
        }
    }
}