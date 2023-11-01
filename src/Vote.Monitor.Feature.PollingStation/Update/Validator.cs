using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Feature.PollingStation.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Address)
            .NotEmpty();

        RuleFor(x => x.Tags)
            .NotEmpty();
    }
}
