﻿namespace Feature.MonitoringObservers.Activate;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.NgoId)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
