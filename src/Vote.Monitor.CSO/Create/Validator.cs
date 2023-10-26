using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.CSO.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .NotEmpty();
    }
}
