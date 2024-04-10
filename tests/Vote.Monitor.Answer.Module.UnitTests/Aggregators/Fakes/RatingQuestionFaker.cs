using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;
using Vote.Monitor.TestUtils.Fakes.Aggregates;

namespace Vote.Monitor.Answer.Module.UnitTests.Aggregators.Fakes;

public sealed class RatingQuestionFaker : Faker<RatingQuestion>
{
    public RatingQuestionFaker(RatingScale? scale = null)
    {
        CustomInstantiator(f => RatingQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(), new TranslatedStringFaker(), scale ?? f.PickRandom<RatingScale>(RatingScale.List)));
    }
}
