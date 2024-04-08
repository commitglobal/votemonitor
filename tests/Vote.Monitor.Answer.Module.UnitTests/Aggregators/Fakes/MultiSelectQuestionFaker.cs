using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class MultiSelectQuestionFaker : Faker<MultiSelectQuestion>
{
    public MultiSelectQuestionFaker(List<SelectOption>? options = null)
    {
        CustomInstantiator(f => MultiSelectQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(), new TranslatedStringFaker(), options ?? []));
    }
}
