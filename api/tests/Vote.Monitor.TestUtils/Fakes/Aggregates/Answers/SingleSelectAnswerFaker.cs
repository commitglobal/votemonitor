using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class SingleSelectAnswerFaker : Faker<SingleSelectAnswer>
{
    public SingleSelectAnswerFaker()
    {
        CustomInstantiator(f => SingleSelectAnswer.Create(f.Random.Guid(), new SelectedOptionFaker().Generate()));
    }
}
