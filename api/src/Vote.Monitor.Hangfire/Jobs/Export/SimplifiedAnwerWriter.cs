using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Extensions;
using Vote.Monitor.Hangfire.Models;

namespace Vote.Monitor.Hangfire.Jobs.Export;

public class SimplifiedAnswerWriter
{
    private readonly List<string> _questionHeader = [];
    private readonly string _defaultLanguage;

    private readonly Dictionary<Guid, List<object>> _submissionsData = new();
    private readonly Dictionary<Guid, List<string>> _submissionsNotes = new();
    private readonly Dictionary<Guid, List<string>> _submissionsAttachments = new();

    public Guid QuestionId => _question.Id;
    private readonly BaseQuestion _question;

    public List<string> Header =>
    [
        .._questionHeader,
        "Notes",
        "Attachments"
    ];

    public SimplifiedAnswerWriter(string defaultLanguage, BaseQuestion question)
    {
        _defaultLanguage = defaultLanguage;
        _question = question;
        WriteHeader(question);
    }

    private void WriteHeader(BaseQuestion question)
    {
        _questionHeader.Add(question.Code + " - " + question.Text[_defaultLanguage]);
        if (question is MultiSelectQuestion multiSelectQuestion)
        {
            if (multiSelectQuestion.Options.Any(x => x.IsFreeText))
            {
                _questionHeader.Add("FreeText");
            }
        }

        if (question is SingleSelectQuestion singleSelectQuestion)
        {
            if (singleSelectQuestion.Options.Any(x => x.IsFreeText))
            {
                _questionHeader.Add("FreeText");
            }
        }
    }


    public void WithSubmission(Guid submissionId, BaseAnswer answer, List<SubmissionAttachmentModel> attachments,
        List<SubmissionNoteModel> notes)
    {
        List<object> data = [];

        switch (answer)
        {
            case DateAnswer dateAnswer:
                data.Add(dateAnswer.Date.ToString("s"));
                break;

            case NumberAnswer numberAnswer:
                data.Add(numberAnswer.Value);
                break;
            case RatingAnswer ratingAnswer:
                data.Add(ratingAnswer.Value);
                break;

            case TextAnswer textAnswer:
                data.Add(textAnswer.Text);
                break;
            case SingleSelectAnswer singleSelectAnswer:
                var singleSelectQuestion = _question as SingleSelectQuestion;
                var selectedOptionText = singleSelectQuestion!.Options
                    .First(x => x.Id == singleSelectAnswer.Selection.OptionId).Text;

                data.Add(selectedOptionText[_defaultLanguage]);

                if (singleSelectQuestion.Options.Any(x => x.IsFreeText))
                {
                    data.Add(singleSelectAnswer.Selection.Text);
                }

                break;
            case MultiSelectAnswer multiSelectAnswer:
                var multiSelectQuestion = _question as MultiSelectQuestion;
                var selectedOptionIds = multiSelectAnswer.Selection.Select(x => x.OptionId);
                var optionTexts = multiSelectQuestion!.Options.Where(x => selectedOptionIds.Contains(x.Id))
                    .Select(x => x.Text[_defaultLanguage]);

                var freeTexts = multiSelectAnswer.Selection.Select(x => x.Text).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                data.Add(string.Join(",", optionTexts));

                if (multiSelectQuestion.Options.Any(x => x.IsFreeText))
                {
                    data.Add(string.Join(",", freeTexts));
                }

                break;
            default:
                throw new ArgumentException();
        }

        _submissionsData.Add(submissionId, data);
        _submissionsNotes.Add(submissionId, notes.Select(x => x.Text).ToList());
        _submissionsAttachments.Add(submissionId, attachments.Select(x => x.PresignedUrl).ToList());
    }


    public List<object> Write(Guid submissionId)
    {
        if (_submissionsData.TryGetValue(submissionId, out var data))
        {
            var notes = _submissionsNotes[submissionId];
            var attachments = _submissionsAttachments[submissionId];

            return
            [
                ..data,
                string.Join("--------------------------\n\n", notes),
                string.Join("--------------------------\n\n", attachments)
            ];
        }

        return Header.Select(_ => string.Empty as object).ToList();
    }
}