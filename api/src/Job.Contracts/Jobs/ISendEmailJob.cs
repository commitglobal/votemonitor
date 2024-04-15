namespace Job.Contracts.Jobs;

public interface ISendEmailJob
{
    Task SendAsync(string to, string subject, string body);
}
