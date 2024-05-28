using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class ResetPasswordEmailGenerator : IEmailGenerator<ResetPasswordEmailProps>
{
    public EmailModel Generate(ResetPasswordEmailProps props)
    {
        var template = EmailTemplateLoader.GetTemplate(EmailTemplateType.ResetPassword);

        var body = template
            .Replace("~$name$~", props.FullName)
            .Replace("~$resetPasswordUrl$~", props.ResetPasswordUrl)
            .Replace("~$cdnUrl$~", props.CdnUrl);

        return new EmailModel("Reset your password on VoteMonitor Platform", body);
    }
}
