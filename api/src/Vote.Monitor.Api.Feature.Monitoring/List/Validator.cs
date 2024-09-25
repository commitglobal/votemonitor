﻿namespace Vote.Monitor.Api.Feature.Monitoring.List;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
    }
}
