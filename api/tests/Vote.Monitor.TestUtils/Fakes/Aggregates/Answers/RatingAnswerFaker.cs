using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class RatingAnswerFaker : Faker<RatingAnswer>
{
    public RatingAnswerFaker(Guid? questionId = null)
    {
        CustomInstantiator(f => RatingAnswer.Create(questionId ?? f.Random.Guid(), f.Random.Number(1, 10)));
    }
}
