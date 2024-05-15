using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

public sealed class SelectOptionFaker : Faker<SelectOption>
{
    public SelectOptionFaker(Guid? id = null, string[]? languageList = null)
    {
        CustomInstantiator(f => SelectOption.Create(id ?? f.Random.Guid(), new TranslatedStringFaker(languageList)));
    }
}
