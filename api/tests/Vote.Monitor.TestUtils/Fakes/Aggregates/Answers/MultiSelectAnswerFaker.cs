using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class MultiSelectAnswerFaker : Faker<MultiSelectAnswer>
{
    public MultiSelectAnswerFaker()
    {
        CustomInstantiator(f => MultiSelectAnswer.Create(f.Random.Guid(), new SelectedOptionFaker().GenerateBetween(1, 4)));
    }
}
