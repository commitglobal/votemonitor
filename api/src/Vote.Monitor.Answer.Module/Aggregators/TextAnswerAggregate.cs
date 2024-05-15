using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.Answer.Module.Aggregators;

public class TextAnswerAggregate(TextQuestion question, int displayOrder) : BaseAnswerAggregate(question, displayOrder)
{
    private readonly List<Response<string>> _answers = new();
    public IReadOnlyList<Response<string>> Answers => _answers.AsReadOnly();

    protected override void QuestionSpecificAggregate(Guid responderId, BaseAnswer answer)
    {
        if (answer is not TextAnswer textAnswer)
        {
            throw new ArgumentException($"Invalid answer received: {answer.Discriminator}", nameof(answer));
        }

        _answers.Add(new Response<string>(responderId, textAnswer.Text));
    }
}
