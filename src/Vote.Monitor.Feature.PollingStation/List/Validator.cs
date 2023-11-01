using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Feature.PollingStation.List;

internal class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.PageNumber).GreaterThan(0);
    }

}
