using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class InvitationExistingUserEmailGenerator : IEmailGenerator<InvitationExistingUserEmailProps>
{
    public string Generate(InvitationExistingUserEmailProps props)
    {
        var template = EmailTemplateLoader.GetTemplate(EmailTemplateType.InvitationExistingUser);

        var mail = template
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$acceptUrl$~", props.AcceptUrl);

        return mail;
    }
}
