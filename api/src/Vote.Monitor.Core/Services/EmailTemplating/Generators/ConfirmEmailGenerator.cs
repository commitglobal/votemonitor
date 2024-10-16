using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class ConfirmEmailGenerator
{
    private static readonly string Template = EmailTemplateLoader.GetTemplate(EmailTemplateType.ConfirmEmail);

    public static EmailModel Generate(ConfirmEmailProps props)
    {
        var body = Template
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$name$~", props.FullName)
            .Replace("~$confirmUrl$~", props.ConfirmUrl);

        return new EmailModel("Confirm your email on VoteMonitor platform", body);
    }
}