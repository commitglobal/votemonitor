using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class InvitationNewUserEmailGenerator : IEmailGenerator<InvitationNewUserEmailProps>
{
    public EmailModel Generate(InvitationNewUserEmailProps props)
    {
        var template = EmailTemplateLoader.GetTemplate(EmailTemplateType.InvitationNewUser);

        var body = template
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$acceptUrl$~", props.AcceptUrl);

        return new EmailModel("Register on VoteMonitor platform to monitor elections", body);

    }
}
