using Vote.Monitor.Core.Services.EmailTemplating.Generators;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating;

internal class EmailTemplateFactory : IEmailTemplateFactory
{
    private static IEmailGenerator<TEmailProps> GetEmailGenerator<TEmailProps>(EmailTemplateType type)
        where TEmailProps : BaseEmailProps
    {
        return type switch
        {
            EmailTemplateType.ConfirmEmail => (IEmailGenerator<TEmailProps>)new ConfirmEmailGenerator(),
            EmailTemplateType.ResetPassword => (IEmailGenerator<TEmailProps>)new ResetPasswordEmailGenerator(),
            EmailTemplateType.InvitationExistingUser => (IEmailGenerator<TEmailProps>)
                new InvitationExistingUserEmailGenerator(),
            EmailTemplateType.InvitationNewUser =>
                (IEmailGenerator<TEmailProps>)new InvitationNewUserEmailGenerator(),
            _ => throw new ArgumentException($"Unmapped email template type {type}", nameof(type))
        };
    }

    public string GenerateEmailTemplate<TEmailProps>(EmailTemplateType templateType, TEmailProps emailProps) where TEmailProps : BaseEmailProps
    {
        var generator = GetEmailGenerator<TEmailProps>(templateType);
        return generator.Generate(emailProps);
    }
}
