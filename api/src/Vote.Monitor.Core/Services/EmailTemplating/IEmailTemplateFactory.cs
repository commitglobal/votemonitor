using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating;

public interface IEmailTemplateFactory
{
    EmailModel GenerateConfirmAccountEmail(ConfirmEmailProps emailProps);

    EmailModel GenerateResetPasswordEmail(ResetPasswordEmailProps emailProps);

    EmailModel GenerateInvitationExistingUserEmail(InvitationExistingUserEmailProps emailProps);

    EmailModel GenerateNewUserInvitationEmail(InvitationNewUserEmailProps emailProps);
    EmailModel GenerateCitizenReportEmail(CitizenReportEmailProps citizenReportEmailProps);
}