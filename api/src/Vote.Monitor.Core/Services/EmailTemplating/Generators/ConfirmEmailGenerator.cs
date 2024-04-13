using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class ConfirmEmailGenerator : IEmailGenerator<ConfirmEmailProps>
{
    public EmailModel Generate(ConfirmEmailProps props)
    {
        var template = EmailTemplateLoader.GetTemplate(EmailTemplateType.ConfirmEmail);

        var body = template
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$email$~", props.Email)
            .Replace("~$confirmUrl$~", props.Url);

        return new EmailModel("Confirm your email on VoteMonitor platform", body);
    }
}
