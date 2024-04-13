using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class TextAnswerFaker : Faker<TextAnswer>
{
    public TextAnswerFaker(Guid? questionId = null)
    {
        CustomInstantiator(f => TextAnswer.Create(questionId ?? f.Random.Guid(), f.Lorem.Text()));
    }
}
