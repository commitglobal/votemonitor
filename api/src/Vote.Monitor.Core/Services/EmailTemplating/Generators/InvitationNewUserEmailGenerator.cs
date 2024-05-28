using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class InvitationNewUserEmailGenerator : IEmailGenerator<InvitationNewUserEmailProps>
{
    public EmailModel Generate(InvitationNewUserEmailProps props)
    {
        var template = EmailTemplateLoader.GetTemplate(EmailTemplateType.InvitationNewUser);

        var body = template
            .Replace("~$name$~", props.FullName)
            .Replace("~$confirmUrl$~", props.AcceptUrl)
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$ngoName$~", props.NgoName)
            .Replace("~$electionRoundDetails$~", props.ElectionRoundDetails);

        return new EmailModel($"{props.NgoName} has invited you to be an observer for {props.ElectionRoundDetails}.", body);

    }
}
