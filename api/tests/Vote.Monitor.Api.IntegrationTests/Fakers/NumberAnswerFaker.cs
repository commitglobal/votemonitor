using Module.Answers.Requests;

namespace Vote.Monitor.Api.IntegrationTests.Fakers;

public sealed class NumberAnswerFaker : Faker<NumberAnswerRequest>
{
    public NumberAnswerFaker(Guid questionId)
    {
        RuleFor(x => x.QuestionId, questionId);
        RuleFor(x => x.Value, f => f.Random.Number(0, 1_000_000));
    }
}
