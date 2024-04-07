using Vote.Monitor.Answer.Module.Aggregators.Extensions;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class SingleSelectAnswerAggregate : BaseAnswerAggregate
{
    private readonly List<Response<SelectedOption>> _answers = new();
    public IReadOnlyList<Response<SelectedOption>> Answers => _answers.AsReadOnly();

    private readonly Dictionary<Guid, int> _answersHistogram;
    public IReadOnlyDictionary<Guid, int> AnswersHistogram => _answersHistogram.AsReadOnly();

    public SingleSelectAnswerAggregate(SingleSelectQuestion question) : base(question.Id)
    {
        _answersHistogram = question.Options.ToDictionary(o => o.Id, _ => 0);
    }

    protected override void QuestionSpecificAggregate(Guid responderId, BaseAnswer answer)
    {
        if (answer is not SingleSelectAnswer singleSelectAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        var response = new Response<SelectedOption>(responderId, singleSelectAnswer.Selection);

        _answers.Add(response);
        _answersHistogram.IncrementFor(singleSelectAnswer.Selection.OptionId);
    }
}
