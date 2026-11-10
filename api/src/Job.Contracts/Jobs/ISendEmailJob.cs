namespace Job.Contracts.Jobs;

public interface ISendEmailJob
{
    /// <summary>HTML only. Kept for jobs enqueued before the plain-text alternative.</summary>
    Task SendAsync(string to, string subject, string body);

    /// <summary>HTML with its plain-text alternative.</summary>
    Task SendAsync(string to, string subject, string body, string textBody);
}
