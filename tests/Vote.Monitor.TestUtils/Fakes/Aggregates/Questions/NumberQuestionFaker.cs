using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

public sealed class NumberQuestionFaker : Faker<NumberQuestion>
{
    public NumberQuestionFaker()
    {
        CustomInstantiator(f => NumberQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(), new TranslatedStringFaker(), new TranslatedStringFaker()));
    }
}
