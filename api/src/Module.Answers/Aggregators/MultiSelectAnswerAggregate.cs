using System.Text.Json.Serialization;
using Module.Answers.Models;
using Module.Answers.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public class MultiSelectAnswerAggregate : BaseAnswerAggregate
{
    private List<TextResponse> _freeTexts = [];

    private Dictionary<Guid, int> _answersHistogram = new();
    private readonly bool _hasFreeTextOption;
    [JsonInclude]
    public IReadOnlyDictionary<Guid, int> AnswersHistogram
    {
        get => _answersHistogram.AsReadOnly();
        private set => _answersHistogram = new Dictionary<Guid, int>(value);
    }
    [JsonInclude]
    public IReadOnlyCollection<TextResponse> FreeTexts
    {
        get => _freeTexts.ToList().AsReadOnly();
        private set => _freeTexts = value.ToList();
    }

    [JsonConstructor]
    private MultiSelectAnswerAggregate()
    {
    }

    public MultiSelectAnswerAggregate(MultiSelectQuestion question, int displayOrder) : base(question, displayOrder)
    {
        _answersHistogram = question.Options.ToDictionary(o => o.Id, _ => 0);
        _hasFreeTextOption = question.Options.Any(o => o.IsFreeText);
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswer answer)
    {
        if (answer is not MultiSelectAnswer multiSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        foreach (var selectedOption in multiSelectAnswer.Selection)
        {
            _answersHistogram.IncrementFor(selectedOption.OptionId);
        }

        var questionOptions = ((MultiSelectQuestion)Question).Options;
        foreach (var selectedOption in multiSelectAnswer.Selection)
        {
            var questionOption = questionOptions.FirstOrDefault(o => o.Id == selectedOption.OptionId);
            if (questionOption != null && questionOption.IsFreeText && !string.IsNullOrWhiteSpace(selectedOption.Text))
            {
                _freeTexts.Add(new TextResponse(submissionId, monitoringObserverId, selectedOption.Text));
            }
        }
    }

    protected override void QuestionSpecificAggregate(Guid submissionId, Guid monitoringObserverId, BaseAnswerModel answer)
    {
        if (answer is not MultiSelectAnswerModel multiSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        foreach (var selectedOption in multiSelectAnswer.Selection)
        {
            _answersHistogram.IncrementFor(selectedOption.OptionId);
        }

        var questionOptions = ((MultiSelectQuestion)Question).Options;
        foreach (var selectedOption in multiSelectAnswer.Selection)
        {
            var questionOption = questionOptions.FirstOrDefault(o => o.Id == selectedOption.OptionId);
            if (questionOption != null && questionOption.IsFreeText && !string.IsNullOrWhiteSpace(selectedOption.Text))
            {
                _freeTexts.Add(new TextResponse(submissionId, monitoringObserverId, selectedOption.Text));
            }
        }
    }
}
