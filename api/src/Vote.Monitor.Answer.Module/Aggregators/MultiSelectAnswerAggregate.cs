using Vote.Monitor.Answer.Module.Aggregators.Extensions;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class MultiSelectAnswerAggregate : BaseAnswerAggregate
{
    private readonly Dictionary<Guid, int> _answersHistogram;
    public IReadOnlyDictionary<Guid, int> AnswersHistogram => _answersHistogram.AsReadOnly();

    public MultiSelectAnswerAggregate(MultiSelectQuestion question, int displayOrder) : base(question, displayOrder)
    {
        _answersHistogram = question.Options.ToDictionary(o => o.Id, _ => 0);
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
    }
}
