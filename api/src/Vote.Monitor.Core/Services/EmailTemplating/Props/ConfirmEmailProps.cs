namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public record ConfirmEmailProps(string CdnUrl, string Email, string Url) : BaseEmailProps(CdnUrl);
