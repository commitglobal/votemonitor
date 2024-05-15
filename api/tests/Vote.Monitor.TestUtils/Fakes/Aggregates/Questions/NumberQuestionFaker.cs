using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

public sealed class NumberQuestionFaker : Faker<NumberQuestion>
{
    public NumberQuestionFaker(string[]? languageList = null)
    {
        CustomInstantiator(f => NumberQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(languageList), new TranslatedStringFaker(languageList), new TranslatedStringFaker(languageList)));
    }
}
