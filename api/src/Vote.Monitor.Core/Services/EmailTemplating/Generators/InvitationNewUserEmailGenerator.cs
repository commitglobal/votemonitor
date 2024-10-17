using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class InvitationNewUserEmailGenerator
{
    private static readonly string Template = EmailTemplateLoader.GetTemplate(EmailTemplateType.NewUserInvitation);

    public static EmailModel Generate(InvitationNewUserEmailProps props)
    {
        var body = Template
            .Replace("~$name$~", props.FullName)
            .Replace("~$confirmUrl$~", props.AcceptUrl)
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$ngoName$~", props.NgoName)
            .Replace("~$electionRoundDetails$~", props.ElectionRoundDetails);

        return new EmailModel($"{props.NgoName} has invited you to be an observer for {props.ElectionRoundDetails}.",
            body);
    }
}