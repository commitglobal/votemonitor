using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Feature.PollingStation.Import;

public class Validator: Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.File)
            .NotEmpty();
    }
}
