using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class InvitationExistingUserEmailGenerator : IEmailGenerator<InvitationExistingUserEmailProps>
{
    public EmailModel Generate(InvitationExistingUserEmailProps props)
    {
        var template = EmailTemplateLoader.GetTemplate(EmailTemplateType.InvitationExistingUser);

        var body = template
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$ngoName$~", props.NgoName)
            .Replace("~$electionRoundDetails$~", props.ElectionRoundDetails);

        return new EmailModel($"Accept invitation to monitor elections", body);

    }
}
