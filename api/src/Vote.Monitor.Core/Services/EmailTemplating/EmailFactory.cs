using Vote.Monitor.Core.Services.EmailTemplating.Generators;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating;

internal class EmailTemplateFactory : IEmailTemplateFactory
{
    public EmailModel GenerateConfirmAccountEmail(ConfirmEmailProps emailProps) =>
        ConfirmEmailGenerator.Generate(emailProps);

    public EmailModel GenerateResetPasswordEmail(ResetPasswordEmailProps emailProps) =>
        ResetPasswordEmailGenerator.Generate(emailProps);

    public EmailModel GenerateInvitationExistingUserEmail(InvitationExistingUserEmailProps emailProps) =>
        InvitationExistingUserEmailGenerator.Generate(emailProps);

    public EmailModel GenerateNewUserInvitationEmail(InvitationNewUserEmailProps emailProps) =>
        InvitationNewUserEmailGenerator.Generate(emailProps);

    public EmailModel GenerateCitizenReportEmail(CitizenReportEmailProps emailProps) =>
        CitizenReportEmailGenerator.Generate(emailProps);
}