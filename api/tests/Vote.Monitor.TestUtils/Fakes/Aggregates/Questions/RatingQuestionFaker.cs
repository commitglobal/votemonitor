﻿using Bogus;
using Vote.Monitor.Domain.Entities.FormBase.Questions;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates.Questions;

public sealed class RatingQuestionFaker : Faker<RatingQuestion>
{
    public RatingQuestionFaker(RatingScale? scale = null)
    {
        CustomInstantiator(f => RatingQuestion.Create(f.Random.Guid(), f.Random.AlphaNumeric(2),
            new TranslatedStringFaker(), new TranslatedStringFaker(), scale ?? f.PickRandom<RatingScale>(RatingScale.List)));
    }
}
