﻿namespace Vote.Monitor.Api.Feature.Auth.Login;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();
    }
}
