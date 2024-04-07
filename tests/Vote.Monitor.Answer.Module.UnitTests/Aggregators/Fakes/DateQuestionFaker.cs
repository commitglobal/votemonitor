using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class DateQuestionFaker : Faker<DateQuestion>
{
    public DateQuestionFaker()
    {
        CustomInstantiator(f => DateQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(), new TranslatedStringFaker()));
    }
}
