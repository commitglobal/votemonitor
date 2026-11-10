namespace Vote.Monitor.Core.Services.EmailTemplating;

/// <param name="TextBody">The plain-text alternative of <paramref name="Body"/>, sent beside it.</param>
public record EmailModel(string Subject, string Body, string TextBody);
