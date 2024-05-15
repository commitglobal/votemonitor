using Vote.Monitor.Core.Services.EmailTemplating.Generators;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating;

internal class EmailTemplateFactory : IEmailTemplateFactory
{
    public EmailModel GenerateEmail(EmailTemplateType templateType, BaseEmailProps emailProps)
    {
        return templateType switch
        {
            EmailTemplateType.ConfirmEmail => new ConfirmEmailGenerator().Generate(emailProps as ConfirmEmailProps),
            EmailTemplateType.ResetPassword => new ResetPasswordEmailGenerator().Generate(emailProps as ResetPasswordEmailProps),
            EmailTemplateType.InvitationExistingUser => new InvitationExistingUserEmailGenerator().Generate(emailProps as InvitationExistingUserEmailProps),
            EmailTemplateType.InvitationNewUser => new InvitationNewUserEmailGenerator().Generate(emailProps as InvitationNewUserEmailProps),
            _ => throw new ArgumentException($"Unmapped email template type {templateType}", nameof(templateType))
        };
    }
}
