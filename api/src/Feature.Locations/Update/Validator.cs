﻿namespace Feature.Locations.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Level1).NotEmpty();
        RuleFor(x => x.Level2).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.Level3));
        RuleFor(x => x.Level3).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.Level4));
        RuleFor(x => x.Level4).NotEmpty().When(x => !string.IsNullOrWhiteSpace(x.Level5));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Tags)
            .Must(filter => { return filter.Keys.All(tag => !string.IsNullOrWhiteSpace(tag)); })
            .When(x => x.Tags != null && x.Tags.Any());
    }
}