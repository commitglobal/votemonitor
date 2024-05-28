namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public record ConfirmEmailProps(string CdnUrl, string FullName, string ConfirmUrl) : BaseEmailProps(CdnUrl);
