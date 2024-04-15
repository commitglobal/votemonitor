using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class DateAnswerFaker : Faker<DateAnswer>
{
    public DateAnswerFaker(Guid? questionId = null)
    {
        CustomInstantiator(f => DateAnswer.Create(questionId ?? f.Random.Guid(), f.Date.Recent()));
    }
}
