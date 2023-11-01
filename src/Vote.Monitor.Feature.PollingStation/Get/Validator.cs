using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Feature.PollingStation.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
