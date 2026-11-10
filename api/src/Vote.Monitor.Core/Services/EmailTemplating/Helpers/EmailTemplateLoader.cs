using System.Text;

namespace Vote.Monitor.Core.Services.EmailTemplating.Helpers;

internal static class EmailTemplateLoader
{
    private static readonly Dictionary<EmailTemplateType, string> _templateMap = new()
    {
        { EmailTemplateType.ConfirmEmail, "confirm-email.html" },
        { EmailTemplateType.ResetPassword, "reset-password.html" },
        { EmailTemplateType.InvitationExistingUser, "invitation-existing-user.html" },
        { EmailTemplateType.NewUserInvitation, "invitation-new-user.html" },
        { EmailTemplateType.CitizenReport, "citizen-report.html" },


        { EmailTemplateType.InputAnswerFragment, "Fragments/input-answer-fragment.html" },
        
        { EmailTemplateType.RatingAnswerFragment, "Fragments/rating-answer-fragment.html" },
        { EmailTemplateType.RatingAnswerOptionOptionFragment, "Fragments/rating-answer-option-fragment.html" },
        { EmailTemplateType.RatingAnswerOptionCheckedOptionFragment, "Fragments/rating-answer-option-selected-fragment.html" },
        
        { EmailTemplateType.SelectAnswerFragment, "Fragments/select-answer-fragment.html" },
        { EmailTemplateType.SelectAnswerOptionFragment, "Fragments/select-answer-option-fragment.html" },
        { EmailTemplateType.SelectAnswerCheckedOptionFragment, "Fragments/select-answer-option-selected-fragment.html" },
        
        { EmailTemplateType.AnswerAttachmentsFragment, "Fragments/answer-attachments-fragment.html" },
        { EmailTemplateType.AttachmentFragment, "Fragments/attachment-fragment.html" },
        
        { EmailTemplateType.AnswerNotesFragment, "Fragments/answer-notes-fragment.html" },
        { EmailTemplateType.NoteFragment, "Fragments/note-fragment.html" }
    };

    public static string GetTemplate(EmailTemplateType templateType) => Read(_templateMap[templateType]);

    /// <summary>The template's plain-text version: the same name with <c>.txt</c>.</summary>
    public static string GetTextTemplate(EmailTemplateType templateType) =>
        Read(Path.ChangeExtension(_templateMap[templateType], ".txt"));

    /// <summary>The HTML or the plain-text template.</summary>
    public static string Get(EmailTemplateType templateType, bool text) =>
        text ? GetTextTemplate(templateType) : GetTemplate(templateType);

    private static string Read(string relativePath)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var tmplFolder = Path.Combine(baseDirectory, "EmailTemplates");
        var filePath = Path.Combine(tmplFolder, relativePath);

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.Default);
        var mailText = sr.ReadToEnd();
        sr.Close();

        return mailText;
    }
}