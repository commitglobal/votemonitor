namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public record ResetPasswordEmailProps(string CdnUrl, string ResetPasswordUrl) : BaseEmailProps(CdnUrl);
