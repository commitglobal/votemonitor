using System.Text.Json.Serialization;
using Module.Answers.Models;
using Module.Answers.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public class SingleSelectAnswerAggregate : BaseAnswerAggregate
{
    private List<TextResponse> _freeTexts = [];
    private Dictionary<Guid, int> _answersHistogram = new();
    [JsonInclude]
    public IReadOnlyDictionary<Guid, int> AnswersHistogram
    {
        get => _answersHistogram.AsReadOnly();
        private set => _answersHistogram = new Dictionary<Guid, int>(value);
    }
    [JsonInclude]
    public IReadOnlyList<TextResponse> FreeTexts
    {
        get => _freeTexts.ToList().AsReadOnly();
        private set => _freeTexts = value.ToList();
    }

    [JsonConstructor]
    private SingleSelectAnswerAggregate()
    {
    }

    public SingleSelectAnswerAggregate(SingleSelectQuestion question, int displayOrder) : base(question, displayOrder)
    {
        _answersHistogram = question.Options.ToDictionary(o => o.Id, _ => 0);
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        if (answer is not SingleSelectAnswer singleSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(singleSelectAnswer.Selection.OptionId);
        
        var selection = singleSelectAnswer.Selection;
        var questionOption = ((SingleSelectQuestion)Question).Options.FirstOrDefault(o => o.Id == selection.OptionId);
        if (questionOption != null && questionOption.IsFreeText && !string.IsNullOrWhiteSpace(selection.Text))
        {
            _freeTexts.Add(new TextResponse(submissionId, monitoringObserverId, selection.Text));
        }
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer)
    {
        if (answer is not SingleSelectAnswerModel singleSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answersHistogram.IncrementFor(singleSelectAnswer.Selection.OptionId);
        
        var selection = singleSelectAnswer.Selection;
        var questionOption = ((SingleSelectQuestion)Question).Options.FirstOrDefault(o => o.Id == selection.OptionId);
        if (questionOption != null && questionOption.IsFreeText && !string.IsNullOrWhiteSpace(selection.Text))
        {
            _freeTexts.Add(new TextResponse(submissionId, monitoringObserverId, selection.Text));
        }
    }
}
