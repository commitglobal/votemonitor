using System.Globalization;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.Hangfire.Extensions;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.FileGenerators;

namespace Vote.Monitor.Hangfire.Jobs.Export;

public class AnswerWriter
{
    private readonly List<string> _questionHeader = [];
    private readonly string _defaultLanguage;

    private readonly Dictionary<Guid, List<object>> _submissionsData = new();
    private readonly Dictionary<Guid, List<string>> _submissionsNotes = new();
    private readonly Dictionary<Guid, List<string>> _submissionsAttachments = new();


    /// <summary>Between the notes, and between the attachments, of one answer: each kind all in one cell.</summary>
    public const string ItemSeparator = "\n\n--------------------\n\n";

    public Guid QuestionId => _question.Id;
    private readonly BaseQuestion _question;

    public List<string> Header =>
    [
        .._questionHeader,
        // One cell for every note of the answer, one for every attachment.
        "Notes",
        "Attachments"
    ];

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
            case TimeQuestion:
            case DateTimeQuestion:
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
            _questionHeader.Add(option.Text[_defaultLanguage] + (option.IsFlagged ? "~$red$~" : ""));

            if (option.IsFreeText)
            {
                _questionHeader.Add(option.Text[_defaultLanguage] + "-UserInput");
            }
        }
    }

    public void WithSubmission(Guid submissionId, BaseAnswer answer, List<SubmissionAttachmentModel> attachments,
        List<NoteModel> notes)
    {
        List<object> data = [];

        switch (answer)
        {
            case DateAnswer dateAnswer:
                data.Add(dateAnswer.Date.ToString("s"));
                break;
            case TimeAnswer timeAnswer:
                data.Add(timeAnswer.Time.ToString("HH:mm", CultureInfo.InvariantCulture));
                break;
            case DateTimeAnswer dateTimeAnswer:
                data.Add(dateTimeAnswer.DateTime.ToString("s"));
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

                var selectedFlagged = singleSelectQuestion.Options.First(x => x.Id == singleSelectAnswer.Selection.OptionId).IsFlagged;
                data.Add(Flag(selectedOptionText[_defaultLanguage], selectedFlagged));

                foreach (var option in singleSelectQuestion!.Options)
                {
                    var selectedOption = singleSelectAnswer.Selection;
                    var isSelected = selectedOption.OptionId == option.Id;
                    // A picked flagged option is red in its own column too.
                    data.Add(isSelected && option.IsFlagged ? Flag(isSelected.ToString(), true) : isSelected);

                    if (option.IsFreeText)
                    {
                        data.Add(selectedOption?.Text ?? string.Empty);
                    }
                }

                break;
            case MultiSelectAnswer multiSelectAnswer:
                var multiSelectQuestion = _question as MultiSelectQuestion;
                var selectedOptionIds = multiSelectAnswer.Selection.Select(x => x.OptionId);
                var optionTexts = multiSelectQuestion!.Options.Where(x => selectedOptionIds.Contains(x.Id))
                    .Select(x => x.Text[_defaultLanguage]);

                var anyFlagged = multiSelectQuestion.Options.Any(x => x.IsFlagged && selectedOptionIds.Contains(x.Id));
                data.Add(Flag(string.Join(",", optionTexts), anyFlagged));

                foreach (var option in multiSelectQuestion!.Options)
                {
                    var selectedOption = multiSelectAnswer.Selection.FirstOrDefault(x => x.OptionId == option.Id);
                    var isSelected = selectedOption != null;
                    data.Add(isSelected && option.IsFlagged ? Flag(isSelected.ToString(), true) : isSelected);

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


    /// <summary>A flagged answer's cell: red in the sheet (ExcelFileGenerator reads the marker).</summary>
    private static object Flag(string text, bool isFlagged) => isFlagged ? ColorMarkers.Red + text : text;

    public List<object> Write(Guid submissionId)
    {
        if (_submissionsData.TryGetValue(submissionId, out var data))
        {
            var notes = _submissionsNotes[submissionId];
            var attachments = _submissionsAttachments[submissionId];

            return
            [
                ..data,
                string.Join(ItemSeparator, notes),
                string.Join(ItemSeparator, attachments)
            ];
        }

        return Header.Select(_ => string.Empty as object).ToList();
    }
}