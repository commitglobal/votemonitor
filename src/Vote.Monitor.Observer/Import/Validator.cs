using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Observer.Import;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CSOId)
            .NotEmpty();
    }
}
