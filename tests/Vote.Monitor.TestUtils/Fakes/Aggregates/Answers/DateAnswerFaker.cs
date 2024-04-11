using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class DateAnswerFaker : Faker<DateAnswer>
{
    public DateAnswerFaker()
    {
        CustomInstantiator(f => DateAnswer.Create(f.Random.Guid(), f.Date.Recent()));
    }
}
