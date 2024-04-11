using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

public sealed class SingleSelectQuestionFaker : Faker<SingleSelectQuestion>
{
    public SingleSelectQuestionFaker(List<SelectOption>? options = null)
    {
        CustomInstantiator(f => SingleSelectQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(), new TranslatedStringFaker(), options ?? []));
    }
}
