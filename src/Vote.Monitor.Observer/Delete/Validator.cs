using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Observer.Delete;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
