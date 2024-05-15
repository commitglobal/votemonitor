using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

public sealed class MultiSelectQuestionFaker : Faker<MultiSelectQuestion>
{
    public MultiSelectQuestionFaker(List<SelectOption>? options = null, string[]? languageList = null)
    {
        CustomInstantiator(f => MultiSelectQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(languageList), options ?? new SelectOptionFaker(languageList: languageList).Generate(4), new TranslatedStringFaker(languageList)));
    }
}
