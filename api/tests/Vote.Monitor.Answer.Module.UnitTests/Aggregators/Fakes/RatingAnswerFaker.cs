using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class RatingAnswerFaker : Faker<RatingAnswer>
{
    public RatingAnswerFaker(Guid? questionId = null, int? value = null)
    {
        CustomInstantiator(f => RatingAnswer.Create(questionId ?? f.Random.Guid(), value ?? f.Random.Int(1, 3)));
    }
}
