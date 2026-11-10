using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal class ConfirmEmailGenerator
{
    private static readonly string Template = EmailTemplateLoader.GetTemplate(EmailTemplateType.ConfirmEmail);
    private static readonly string TextTemplate = EmailTemplateLoader.GetTextTemplate(EmailTemplateType.ConfirmEmail);

    public static EmailModel Generate(ConfirmEmailProps props)
    {
        string Fill(string template) => template
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$name$~", props.FullName)
            .Replace("~$confirmUrl$~", props.ConfirmUrl)
            .Replace("~$year$~", DateTime.UtcNow.Year.ToString());

        return new EmailModel("Confirm your email on VoteMonitor platform", Fill(Template), Fill(TextTemplate));
    }
}