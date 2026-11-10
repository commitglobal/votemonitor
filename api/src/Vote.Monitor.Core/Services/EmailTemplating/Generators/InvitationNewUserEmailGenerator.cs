using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class InvitationNewUserEmailGenerator
{
    private static readonly string Template = EmailTemplateLoader.GetTemplate(EmailTemplateType.NewUserInvitation);
    private static readonly string TextTemplate = EmailTemplateLoader.GetTextTemplate(EmailTemplateType.NewUserInvitation);

    public static EmailModel Generate(InvitationNewUserEmailProps props)
    {
        string Fill(string template) => template
            .Replace("~$name$~", props.FullName)
            .Replace("~$confirmUrl$~", props.AcceptUrl)
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$ngoName$~", props.NgoName)
            .Replace("~$electionRoundDetails$~", props.ElectionRoundDetails)
            .Replace("~$year$~", DateTime.UtcNow.Year.ToString());

        return new EmailModel($"{props.NgoName} has invited you to be an observer for {props.ElectionRoundDetails}.", Fill(Template), Fill(TextTemplate));
    }
}