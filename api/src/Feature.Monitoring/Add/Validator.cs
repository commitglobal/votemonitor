﻿namespace Vote.Monitor.Api.Feature.Monitoring.Add;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.NgoId)
            .NotEmpty();
    }
}
