namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public record ResetPasswordEmailProps(string CdnUrl, string ResetUrl) : BaseEmailProps(CdnUrl);
