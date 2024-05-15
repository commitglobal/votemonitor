namespace Vote.Monitor.Core.Services.EmailTemplating.Props;

public record InvitationExistingUserEmailProps(string CdnUrl, string NgoName, string ElectionRoundDetails) : BaseEmailProps(CdnUrl);
