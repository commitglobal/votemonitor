using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

public sealed class DateQuestionFaker : Faker<DateQuestion>
{
    public DateQuestionFaker(string[]? languageList = null)
    {
        CustomInstantiator(f => DateQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(languageList), new TranslatedStringFaker(languageList)));
    }
}
