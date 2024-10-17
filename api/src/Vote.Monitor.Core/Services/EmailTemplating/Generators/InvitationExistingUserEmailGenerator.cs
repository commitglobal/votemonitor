using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class InvitationExistingUserEmailGenerator
{
    private static readonly string Template = EmailTemplateLoader.GetTemplate(EmailTemplateType.InvitationExistingUser);

    public static EmailModel Generate(InvitationExistingUserEmailProps props)
    {
        var body = Template
            .Replace("~$name$~", props.FullName)
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$ngoName$~", props.NgoName)
            .Replace("~$electionRoundDetails$~", props.ElectionRoundDetails);

        return new EmailModel($"{props.NgoName} has invited you to be an observer for {props.ElectionRoundDetails}.",
            body);
    }
}