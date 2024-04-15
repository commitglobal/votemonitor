namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public record InvitationExistingUserEmailProps(string CdnUrl, string AcceptUrl) : BaseEmailProps(CdnUrl);
