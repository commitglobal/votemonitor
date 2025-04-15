using Module.Answers.Requests;

namespace Vote.Monitor.Api.IntegrationTests.Fakers;

public sealed class RatingAnswerFaker : Faker<RatingAnswerRequest>
{
    public RatingAnswerFaker(Guid questionId)
    {
        RuleFor(x => x.QuestionId, questionId);
        RuleFor(x => x.Value, f => f.Random.Number(1, 10));
    }
}
