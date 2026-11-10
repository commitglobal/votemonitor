using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class ResetPasswordEmailGenerator
{
    private static readonly string Template = EmailTemplateLoader.GetTemplate(EmailTemplateType.ResetPassword);
    private static readonly string TextTemplate = EmailTemplateLoader.GetTextTemplate(EmailTemplateType.ResetPassword);

    public static EmailModel Generate(ResetPasswordEmailProps props)
    {
        string Fill(string template) => template
            .Replace("~$name$~", props.FullName)
            .Replace("~$resetPasswordUrl$~", props.ResetPasswordUrl)
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$year$~", DateTime.UtcNow.Year.ToString());

        return new EmailModel("Reset your password on VoteMonitor Platform", Fill(Template), Fill(TextTemplate));
    }
}