﻿namespace Vote.Monitor.Api.Feature.ElectionRound.List;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(10)
            .LessThanOrEqualTo(100);
    }
}
