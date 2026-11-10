namespace Vote.Monitor.Core.Services.Mailing.Contracts;

/// <summary>
/// Represents an email message request payload.
/// </summary>
/// <param name="textBody">The plain-text alternative of the HTML <paramref name="body"/>; null sends HTML only.</param>
public class MailRequest(string to, string subject, string body, string? textBody = null)
{
    public string To { get; } = to;

    public string Subject { get; } = subject;

    public string Body { get; } = body;

    public string? TextBody { get; } = textBody;
}
