using System.Text;

namespace Vote.Monitor.Core.Services.EmailTemplating.Helpers;

internal static class EmailTemplateLoader
{
    private static readonly Dictionary<EmailTemplateType, string> _templateMap = new()
    {
        { EmailTemplateType.ConfirmEmail, "confirm-email.html" },
        { EmailTemplateType.ResetPassword, "reset-password.html" },
        { EmailTemplateType.InvitationExistingUser, "invitation-existing-user.html" },
        { EmailTemplateType.InvitationNewUser, "invitation-new-user.html" }
    };

    public static string GetTemplate(EmailTemplateType templateType)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var tmplFolder = Path.Combine(baseDirectory, "EmailTemplates");
        var filePath = Path.Combine(tmplFolder, _templateMap[templateType]);

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        var mailText = sr.ReadToEnd();
        sr.Close();

        return mailText;
    }
}
