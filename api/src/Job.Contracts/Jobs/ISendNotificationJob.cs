namespace Job.Contracts.Jobs;

public interface ISendNotificationJob
{
    Task SendAsync(List<string> userIdentifiers, string title, string body, CancellationToken ct = default);
}