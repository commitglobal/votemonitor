using Module.Answers.Models;
using Module.Answers.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Module.Answers.Aggregators;

public class SingleSelectAnswerAggregate : BaseAnswerAggregate
{
    private readonly List<TextResponse> _freeTexts = new();
    private readonly Dictionary<Guid, int> _answersHistogram;
    public IReadOnlyDictionary<Guid, int> AnswersHistogram => _answersHistogram.AsReadOnly();
    public IReadOnlyList<TextResponse> FreeTexts => _freeTexts.ToList().AsReadOnly();

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
