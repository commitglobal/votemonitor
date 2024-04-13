using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class NumberAnswerFaker : Faker<NumberAnswer>
{
    public NumberAnswerFaker(Guid? questionId = null)
    {
        CustomInstantiator(f => NumberAnswer.Create(questionId ?? f.Random.Guid(), f.Random.Number(1, 100)));
    }
}
