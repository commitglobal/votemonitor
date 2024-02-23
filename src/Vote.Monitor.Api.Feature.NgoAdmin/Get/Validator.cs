﻿namespace Vote.Monitor.Api.Feature.NgoAdmin.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.NgoId)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
