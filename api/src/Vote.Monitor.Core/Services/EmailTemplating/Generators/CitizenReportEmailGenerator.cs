using System.Text;
using Vote.Monitor.Core.Services.EmailTemplating.Helpers;
using Vote.Monitor.Core.Services.EmailTemplating.Props;

namespace Vote.Monitor.Core.Services.EmailTemplating.Generators;

internal static class CitizenReportEmailGenerator
{
    private static readonly string Template = EmailTemplateLoader.GetTemplate(EmailTemplateType.CitizenReport);

    public static EmailModel Generate(CitizenReportEmailProps props)
    {
        var body = Template
            .Replace("~$heading$~", props.Heading)
            .Replace("~$preview$~", props.Preview)
            .Replace("~$cdnUrl$~", props.CdnUrl)
            .Replace("~$answers$~", BuildAnswersFragment(props.Answers));

        return new EmailModel(props.Title, body);
    }

    private static string BuildAnswersFragment(IEnumerable<BaseAnswerFragmentProps> answers)
    {
        var userAnswers = answers as BaseAnswerFragmentProps[] ?? answers.ToArray();

        if (!userAnswers.Any()) return string.Empty;

        var result = new StringBuilder();
        foreach (var answer in userAnswers)
        {
            var notesFragment = BuildNotesFragment(answer.NotesListTitle, answer.Notes);
            var attachmentsFragment = BuildAttachmentsFragment(answer.AttachmentsListTitle, answer.Attachments);

            switch (answer)
            {
                case InputAnswerFragmentProps inputAnswer:
                    result.Append(EmailTemplateLoader
                        .GetTemplate(EmailTemplateType.InputAnswerFragment)
                        .Replace("~$text$~", inputAnswer.Text)
                        .Replace("~$answer$~", inputAnswer.Answer)
                        .Replace("~$notes$~", notesFragment)
                        .Replace("~$attachments$~", attachmentsFragment));
                    break;

                case RatingAnswerFragmentProps ratingAnswer:
                    result.Append(EmailTemplateLoader
                        .GetTemplate(EmailTemplateType.RatingAnswerFragment)
                        .Replace("~$text$~", ratingAnswer.Text)
                        .Replace("~$options$~", BuildRatingOptionsFragment(ratingAnswer.Scale, ratingAnswer.Value))
                        .Replace("~$notes$~", notesFragment)
                        .Replace("~$attachments$~", attachmentsFragment));
                    break;

                case SelectAnswerFragmentProps selectAnswer:
                    result.Append(EmailTemplateLoader
                        .GetTemplate(EmailTemplateType.SelectAnswerFragment)
                        .Replace("~$text$~", selectAnswer.Text)
                        .Replace("~$options$~", BuildSelectOptionsFragment(selectAnswer.Options))
                        .Replace("~$notes$~", notesFragment)
                        .Replace("~$attachments$~", attachmentsFragment));
                    break;
            }
        }

        return result.ToString();
    }

    private static string? BuildAttachmentsFragment(string attachmentsListTitle,
        IEnumerable<AttachmentFragmentProps> attachments)
    {
        var attachmentsArray = attachments as AttachmentFragmentProps[] ?? attachments.ToArray();
        if (!attachmentsArray.Any())
        {
            return string.Empty;
        }

        var attachmentsFragments = new StringBuilder();
        var noteFragment = EmailTemplateLoader.GetTemplate(EmailTemplateType.AttachmentFragment);
        foreach (var note in attachmentsArray)
        {
            attachmentsFragments.Append(noteFragment
                .Replace("~$link$~", note.Url)
                .Replace("~$linkText$~", note.Title));
        }

        return EmailTemplateLoader.GetTemplate(EmailTemplateType.AnswerAttachmentsFragment)
            .Replace("~$listTitle$~", attachmentsListTitle)
            .Replace("~$attachments$~", attachmentsFragments.ToString());
    }

    private static string? BuildNotesFragment(string notesListTitle, IEnumerable<NoteFragmentProps> notes)
    {
        var notesArray = notes as NoteFragmentProps[] ?? notes.ToArray();
        if (!notesArray.Any())
        {
            return string.Empty;
        }

        var notesFragments = new StringBuilder();
        var noteFragment = EmailTemplateLoader.GetTemplate(EmailTemplateType.NoteFragment);
        foreach (var note in notesArray)
        {
            notesFragments.Append(noteFragment
                .Replace("~$number$~", note.Index)
                .Replace("~$text$~", note.Text));
        }

        return EmailTemplateLoader.GetTemplate(EmailTemplateType.AnswerNotesFragment)
            .Replace("~$listTitle$~", notesListTitle)
            .Replace("~$notes$~", notesFragments.ToString());
    }

    private static string BuildSelectOptionsFragment(IEnumerable<SelectAnswerOptionFragmentProps> options)
    {
        var result = new StringBuilder();

        foreach (var option in options)
        {
            var optionTemplate = EmailTemplateLoader.GetTemplate(option.IsSelected
                ? EmailTemplateType.SelectAnswerCheckedOptionFragment
                : EmailTemplateType.SelectAnswerOptionFragment);

            result.Append(optionTemplate.Replace("~$text$~", option.Text));
        }

        return result.ToString();
    }

    private static string BuildRatingOptionsFragment(int scale, int value)
    {
        var result = new StringBuilder();

        for (var i = 1; i <= scale; i++)
        {
            var optionTemplate = EmailTemplateLoader.GetTemplate(i == value
                ? EmailTemplateType.RatingAnswerOptionCheckedOptionFragment
                : EmailTemplateType.RatingAnswerOptionOptionFragment);

            result.Append(optionTemplate.Replace("~$value$~", i.ToString()));
        }

        return result.ToString();
    }
}