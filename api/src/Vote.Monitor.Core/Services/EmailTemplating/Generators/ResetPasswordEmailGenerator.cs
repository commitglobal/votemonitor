using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class ResetPasswordEmailGenerator
{
    private static readonly string Template = EmailTemplateLoader.GetTemplate(EmailTemplateType.ResetPassword);

    public static EmailModel Generate(ResetPasswordEmailProps props)
    {
        var body = Template
            .Replace("~$name$~", props.FullName)
            .Replace("~$resetPasswordUrl$~", props.ResetPasswordUrl)
            .Replace("~$cdnUrl$~", props.CdnUrl);

        return new EmailModel("Reset your password on VoteMonitor Platform", body);
    }
}