using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class ResetPasswordEmailGenerator : IEmailGenerator<ResetPasswordEmailProps>
{
    public string Generate(ResetPasswordEmailProps props)
    {
        var template = EmailTemplateLoader.GetTemplate(EmailTemplateType.ResetPassword);

        var mail = template
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$acceptUrl$~", props.ResetUrl);

        return mail;
    }
}
