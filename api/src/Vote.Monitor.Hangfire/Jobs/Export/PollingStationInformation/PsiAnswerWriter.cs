using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Hangfire.Jobs.Export.PollingStationInformation;

public class PsiAnswerWriter
{
    private readonly List<string> _header = [];
    private readonly string _defaultLanguage;
    private readonly BaseQuestion _question;
    private readonly Dictionary<Guid, List<object>> _submissionsData = new();

    public Guid QuestionId => _question.Id;
    public List<string> Header => _header;

    public PsiAnswerWriter(string defaultLanguage, BaseQuestion question)
    {
        _defaultLanguage = defaultLanguage;
        _question = question;
        WriteHeader(question);
    }

    private void WriteHeader(BaseQuestion question)
    {
        _header.Add($"{question.Code} - {question.Text[_defaultLanguage]}");

        if (question is SingleSelectQuestion single && single.Options.Any(x => x.IsFreeText))
        {
            _header.Add($"{question.Code} - FreeText");
        }

        if (question is MultiSelectQuestion multi && multi.Options.Any(x => x.IsFreeText))
        {
            _header.Add($"{question.Code} - FreeText");
        }
    }

    public void WithSubmission(Guid submissionId, BaseAnswer answer)
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
            {
                var singleSelectQuestion = (SingleSelectQuestion)_question;
                var selectedOptionText = singleSelectQuestion.Options
                    .First(x => x.Id == singleSelectAnswer.Selection.OptionId).Text;
                data.Add(selectedOptionText[_defaultLanguage]);

                if (singleSelectQuestion.Options.Any(x => x.IsFreeText))
                {
                    data.Add(singleSelectAnswer.Selection.Text ?? string.Empty);
                }

                break;
            }
            case MultiSelectAnswer multiSelectAnswer:
            {
                var multiSelectQuestion = (MultiSelectQuestion)_question;
                var selectedOptionIds = multiSelectAnswer.Selection.Select(x => x.OptionId).ToHashSet();
                var optionTexts = multiSelectQuestion.Options
                    .Where(x => selectedOptionIds.Contains(x.Id))
                    .Select(x => x.Text[_defaultLanguage]);
                data.Add(string.Join(",", optionTexts));

                if (multiSelectQuestion.Options.Any(x => x.IsFreeText))
                {
                    var freeTexts = multiSelectAnswer.Selection
                        .Select(x => x.Text)
                        .Where(x => !string.IsNullOrWhiteSpace(x));
                    data.Add(string.Join(",", freeTexts));
                }

                break;
            }
            default:
                throw new ArgumentException($"Unsupported answer type: {answer.GetType().Name}");
        }

        _submissionsData[submissionId] = data;
    }

    public List<object> Write(Guid submissionId)
    {
        if (_submissionsData.TryGetValue(submissionId, out var data))
        {
            return data;
        }

        return _header.Select(_ => (object)string.Empty).ToList();
    }
}
