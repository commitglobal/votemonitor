using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class NumberAnswerFaker : Faker<NumberAnswer>
{
    public NumberAnswerFaker(Guid? questionId = null, int? value = null)
    {
        CustomInstantiator(f => NumberAnswer.Create(questionId ?? f.Random.Guid(), value ?? f.Random.Int(1, 40)));
    }
}
