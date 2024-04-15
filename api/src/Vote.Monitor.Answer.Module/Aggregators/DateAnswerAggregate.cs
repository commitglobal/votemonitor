using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class DateAnswerAggregate(DateQuestion question) : BaseAnswerAggregate(question.Id)
{
    private readonly List<Response<DateTime>> _answers = new();
    public IReadOnlyList<Response<DateTime>> Answers => _answers.AsReadOnly();

    protected override void QuestionSpecificAggregate(Guid responderId, BaseAnswer answer)
    {
        if (answer is not DateAnswer dateAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answers.Add(new Response<DateTime>(responderId, dateAnswer.Date));
    }
}
