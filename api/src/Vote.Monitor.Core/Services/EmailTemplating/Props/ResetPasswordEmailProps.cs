namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public record ResetPasswordEmailProps(string FullName, string CdnUrl, string ResetPasswordUrl) : BaseEmailProps(CdnUrl);
