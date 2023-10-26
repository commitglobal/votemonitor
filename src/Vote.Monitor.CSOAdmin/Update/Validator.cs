using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.CSOAdmin.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3);
    }
}
