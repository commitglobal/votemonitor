namespace Vote.Monitor.Core.Services.Mailing.Contracts;

public interface IMailService
{
    Task SendAsync(MailRequest request, CancellationToken ct);
}
