﻿namespace Vote.Monitor.Api.Feature.Ngo.Deactivate;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
