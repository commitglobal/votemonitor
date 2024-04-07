using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class TextQuestionFaker : Faker<TextQuestion>
{
    public TextQuestionFaker()
    {
        CustomInstantiator(f => TextQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(), new TranslatedStringFaker(), new TranslatedStringFaker()));
    }
}
