namespace Vote.Monitor.Core.Services.EmailTemplating;

/// <summary>
/// Represents the email content used by the templating service.
/// </summary>
/// <param name="Subject">The email subject line.</param>
/// <param name="Body">The HTML body content of the email.</param>
/// <param name="TextBody">The plain-text alternative of <paramref name="Body"/>, sent beside it.</param>
public record EmailModel(string Subject, string Body, string TextBody);
