namespace Vote.Monitor.Core.Services.Security;

public interface IHtmlStringSanitizer
{
    string Sanitize(string html);
}