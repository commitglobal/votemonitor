using Vote.Monitor.Answer.Module.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class MultiSelectAnswerAggregate : BaseAnswerAggregate
{
    private readonly List<Response<IReadOnlyList<SelectedOption>>> _answers = new();
    public IReadOnlyList<Response<IReadOnlyList<SelectedOption>>> Answers => _answers.AsReadOnly();

    private readonly Dictionary<Guid, int> _answersHistogram;
    public IReadOnlyDictionary<Guid, int> AnswersHistogram => _answersHistogram.AsReadOnly();

    public MultiSelectAnswerAggregate(MultiSelectQuestion question, int displayOrder) : base(question, displayOrder)
    {
        _answersHistogram = question.Options.ToDictionary(o => o.Id, _ => 0);
    }

    protected override void QuestionSpecificAggregate(Guid responderId, BaseAnswer answer)
    {
        if (answer is not MultiSelectAnswer multiSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        var response = new Response<IReadOnlyList<SelectedOption>>(responderId, multiSelectAnswer.Selection);
        _answers.Add(response);

        foreach (var selectedOption in multiSelectAnswer.Selection)
        {
            _answersHistogram.IncrementFor(selectedOption.OptionId);
        }
    }
}
