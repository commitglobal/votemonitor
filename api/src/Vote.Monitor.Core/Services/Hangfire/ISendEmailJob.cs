namespace Vote.Monitor.Core.Services.Hangfire;

public interface ISendEmailJob
{
    Task SendAsync(string to, string subject, string body);
}
