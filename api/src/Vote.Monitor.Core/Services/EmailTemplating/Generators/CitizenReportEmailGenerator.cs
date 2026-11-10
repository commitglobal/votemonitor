using System.Text;
using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal static class CitizenReportEmailGenerator
{
    public static EmailModel Generate(CitizenReportEmailProps props) =>
        new(props.Title, Build(props, text: false), Build(props, text: true));

    /// <summary>The email in HTML, or (<paramref name="text"/>) its plain-text version from the .txt templates.</summary>
    private static string Build(CitizenReportEmailProps props, bool text) =>
        EmailTemplateLoader.Get(EmailTemplateType.CitizenReport, text)
            .Replace("~$heading$~", props.Heading)
            .Replace("~$preview$~", props.Preview)
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$year$~", DateTime.UtcNow.Year.ToString())
            .Replace("~$answers$~", BuildAnswersFragment(props.Answers, text));

    private static string BuildAnswersFragment(IEnumerable<BaseAnswerFragmentProps> answers, bool text)
    {
        var userAnswers = answers as BaseAnswerFragmentProps[] ?? answers.ToArray();

        if (!userAnswers.Any()) return string.Empty;

        var result = new StringBuilder();
        foreach (var answer in userAnswers)
        {
            var notesFragment = BuildNotesFragment(answer.NotesListTitle, answer.Notes, text);
            var attachmentsFragment = BuildAttachmentsFragment(answer.AttachmentsListTitle, answer.Attachments, text);

            switch (answer)
            {
                case InputAnswerFragmentProps inputAnswer:
                    result.Append(EmailTemplateLoader
                        .Get(EmailTemplateType.InputAnswerFragment, text)
                        .Replace("~$text$~", inputAnswer.Text)
                        .Replace("~$answer$~", inputAnswer.Answer)
                        .Replace("~$notes$~", notesFragment)
                        .Replace("~$attachments$~", attachmentsFragment));
                    break;

                case RatingAnswerFragmentProps ratingAnswer:
                    result.Append(EmailTemplateLoader
                        .Get(EmailTemplateType.RatingAnswerFragment, text)
                        .Replace("~$text$~", ratingAnswer.Text)
                        .Replace("~$options$~", BuildRatingOptionsFragment(ratingAnswer.Scale, ratingAnswer.Value, text).TrimEnd())
                        .Replace("~$notes$~", notesFragment)
                        .Replace("~$attachments$~", attachmentsFragment));
                    break;

                case SelectAnswerFragmentProps selectAnswer:
                    result.Append(EmailTemplateLoader
                        .Get(EmailTemplateType.SelectAnswerFragment, text)
                        .Replace("~$text$~", selectAnswer.Text)
                        .Replace("~$options$~", BuildSelectOptionsFragment(selectAnswer.Options, text))
                        .Replace("~$notes$~", notesFragment)
                        .Replace("~$attachments$~", attachmentsFragment));
                    break;
            }
        }

        return result.ToString();
    }

    private static string? BuildAttachmentsFragment(string attachmentsListTitle,
        IEnumerable<AttachmentFragmentProps> attachments, bool text)
    {
        var attachmentsArray = attachments as AttachmentFragmentProps[] ?? attachments.ToArray();
        if (!attachmentsArray.Any())
        {
            return string.Empty;
        }

        var attachmentsFragments = new StringBuilder();
        var noteFragment = EmailTemplateLoader.Get(EmailTemplateType.AttachmentFragment, text);
        foreach (var note in attachmentsArray)
        {
            attachmentsFragments.Append(noteFragment
                .Replace("~$link$~", note.Url)
                .Replace("~$linkText$~", note.Title));
        }

        return EmailTemplateLoader.Get(EmailTemplateType.AnswerAttachmentsFragment, text)
            .Replace("~$listTitle$~", attachmentsListTitle)
            .Replace("~$attachments$~", attachmentsFragments.ToString());
    }

    private static string? BuildNotesFragment(string notesListTitle, IEnumerable<NoteFragmentProps> notes, bool text)
    {
        var notesArray = notes as NoteFragmentProps[] ?? notes.ToArray();
        if (!notesArray.Any())
        {
            return string.Empty;
        }

        var notesFragments = new StringBuilder();
        var noteFragment = EmailTemplateLoader.Get(EmailTemplateType.NoteFragment, text);
        foreach (var note in notesArray)
        {
            notesFragments.Append(noteFragment
                .Replace("~$number$~", note.Index)
                .Replace("~$text$~", note.Text));
        }

        return EmailTemplateLoader.Get(EmailTemplateType.AnswerNotesFragment, text)
            .Replace("~$listTitle$~", notesListTitle)
            .Replace("~$notes$~", notesFragments.ToString());
    }

    private static string BuildSelectOptionsFragment(IEnumerable<SelectAnswerOptionFragmentProps> options, bool text)
    {
        var result = new StringBuilder();

        foreach (var option in options)
        {
            var optionTemplate = EmailTemplateLoader.Get(option.IsSelected
                ? EmailTemplateType.SelectAnswerCheckedOptionFragment
                : EmailTemplateType.SelectAnswerOptionFragment, text);

            result.Append(optionTemplate.Replace("~$text$~", option.Text));
        }

        return result.ToString();
    }

    private static string BuildRatingOptionsFragment(int scale, int value, bool text)
    {
        var result = new StringBuilder();

        for (var i = 1; i <= scale; i++)
        {
            var optionTemplate = EmailTemplateLoader.Get(i == value
                ? EmailTemplateType.RatingAnswerOptionCheckedOptionFragment
                : EmailTemplateType.RatingAnswerOptionOptionFragment, text);

            result.Append(optionTemplate.Replace("~$value$~", i.ToString()));
        }

        return result.ToString();
    }
}