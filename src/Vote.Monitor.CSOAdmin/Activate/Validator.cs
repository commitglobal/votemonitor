using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.CSOAdmin.Activate;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CSOId)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
