using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Observer.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
