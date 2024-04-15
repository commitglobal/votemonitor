using Bogus;
using SubmissionsFaker.Clients.MonitoringObserver.Models;

namespace SubmissionsFaker.Fakers;

public sealed class TextAnswerFaker : Faker<TextAnswerRequest>
{
    public TextAnswerFaker(Guid questionId)
    {
        RuleFor(x => x.QuestionId, questionId);
        RuleFor(x => x.Text, f => f.Lorem.Sentence(100));
    }
}
