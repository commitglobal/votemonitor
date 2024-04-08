using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class DateAnswerFaker : Faker<DateAnswer>
{
    public DateAnswerFaker(Guid? questionId = null, DateTime? value = null)
    {
        CustomInstantiator(f => DateAnswer.Create(questionId ?? f.Random.Guid(), value ?? f.Date.Recent()));
    }
}
