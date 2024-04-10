using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class SelectOptionFaker : Faker<SelectOption>
{
    public SelectOptionFaker(Guid? id = null)
    {
        CustomInstantiator(f => SelectOption.Create(id ?? f.Random.Guid(), new TranslatedStringFaker(), false, false));
    }
}
