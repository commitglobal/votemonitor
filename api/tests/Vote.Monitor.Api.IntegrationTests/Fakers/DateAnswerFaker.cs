using Module.Answers.Requests;

namespace Vote.Monitor.Api.IntegrationTests.Fakers;

public sealed class DateAnswerFaker : Faker<DateAnswerRequest>
{
    public DateAnswerFaker(Guid questionId)
    {
        RuleFor(x => x.QuestionId, questionId);
        RuleFor(x => x.Date, f => f.Date.Recent());
    }
}
