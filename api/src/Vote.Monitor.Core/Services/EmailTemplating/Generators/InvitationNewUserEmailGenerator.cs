using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class InvitationNewUserEmailGenerator : IEmailGenerator<InvitationNewUserEmailProps>
{
    public string Generate(InvitationNewUserEmailProps props)
    {
        var template = EmailTemplateLoader.GetTemplate(EmailTemplateType.InvitationNewUser);

        var mail = template
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$acceptUrl$~", props.AcceptUrl);

        return mail;
    }
}
