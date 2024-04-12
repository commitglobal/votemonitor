using Bogus;
using Vote.Monitor.Domain.Entities.FormAnswerBase.Answers;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Answers;

public sealed class SelectedOptionFaker : Faker<SelectedOption>
{
    public SelectedOptionFaker()
    {
        CustomInstantiator(f => SelectedOption.Create(f.Random.Guid(), f.Lorem.Text()));
    }
}
