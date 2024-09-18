using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Extensions;
using Vote.Monitor.Hangfire.Jobs.Export.FormSubmissions.ReadModels;
using Vote.Monitor.Hangfire.Models;

namespace Vote.Monitor.Hangfire.Jobs.Export;

public class AnswerWriter
{
    private readonly List<string> _questionHeader = [];
    private readonly string _defaultLanguage;

    private readonly Dictionary<Guid, List<object>> _submissionsData = new();
    private readonly Dictionary<Guid, List<string>> _submissionsNotes = new();
    private readonly Dictionary<Guid, List<string>> _submissionsAttachments = new();

    private int _maxNumberOfAttachments = 0;
    private int _maxNumberOfNotes = 0;

    public Guid QuestionId => _question.Id;
    private readonly BaseQuestion _question;

    public List<string> Header =>
    [
        .._questionHeader,
        "Notes",
        ..GetNotesHeader(),
        "Attachments",
        ..GetAttachmentsHeader()
    ];

    private List<string> GetNotesHeader()
    {
        return Enumerable.Range(1, _maxNumberOfNotes).Select(x => $"Note {x}").ToList();
    }

    private List<string> GetAttachmentsHeader()
    {
        return Enumerable.Range(1, _maxNumberOfAttachments).Select(x => $"Attachment {x}").ToList();
    }

    public AnswerWriter(string defaultLanguage, BaseQuestion question)
    {
        _defaultLanguage = defaultLanguage;
        _question = question;
        WriteHeader(question);
    }

    private void WriteHeader(BaseQuestion question)
    {
        _questionHeader.Add(question.Code + " - " + question.Text[_defaultLanguage]);

        switch (question)
        {
            case DateQuestion:
            case TextQuestion:
            case NumberQuestion:
            case RatingQuestion:
                break;

            case MultiSelectQuestion multiSelectQuestion:
                WriteSelectOptions(multiSelectQuestion.Options);
                break;

            case SingleSelectQuestion singleSelectQuestion:
                WriteSelectOptions(singleSelectQuestion.Options);
                break;

            default:
                throw new ArgumentException();
        }
    }

    private void WriteSelectOptions(IReadOnlyList<SelectOption> options)
    {
        foreach (var option in options)
        {
            _questionHeader.Add(option.Text[_defaultLanguage]+ (option.IsFlagged ? "~$red$~" : ""));

            if (option.IsFreeText)
            {
                _questionHeader.Add(option.Text[_defaultLanguage] + "-UserInput");
            }
        }
    }

    public void WithSubmission(Guid submissionId, BaseAnswer answer, List<SubmissionAttachmentModel> attachments, List<SubmissionNoteModel> notes)
    {
        _maxNumberOfAttachments = Math.Max(_maxNumberOfAttachments, attachments.Count);
        _maxNumberOfNotes = Math.Max(_maxNumberOfNotes, notes.Count);
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
                data.Add(string.Empty); // empty cell value in column with the question

                foreach (var option in singleSelectQuestion!.Options)
                {
                    var selectedOption = singleSelectAnswer.Selection;
                    data.Add(selectedOption.OptionId == option.Id);

                    if (option.IsFreeText)
                    {
                        data.Add(selectedOption?.Text ?? string.Empty);
                    }
                }
                break;
            case MultiSelectAnswer multiSelectAnswer:
                var multiSelectQuestion = _question as MultiSelectQuestion;
                data.Add(string.Empty); // empty cell value in column with the question

                foreach (var option in multiSelectQuestion!.Options)
                {
                    var selectedOption = multiSelectAnswer.Selection.FirstOrDefault(x => x.OptionId == option.Id);
                    data.Add(selectedOption != null);

                    if (option.IsFreeText)
                    {
                        data.Add(selectedOption?.Text ?? string.Empty);
                    }
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
                notes.Count,
                ..notes.PadListToTheRight(_maxNumberOfNotes, string.Empty),
                attachments.Count,
                ..attachments.PadListToTheRight(_maxNumberOfAttachments, string.Empty)
            ];
        }

        return Header.Select(_ => string.Empty as object).ToList();
    }
}
