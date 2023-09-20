using FastEndpoints;
using FluentValidation;

namespace Vote.Monitor.Feature.Example.Greet;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("your name is required!")
            .MinimumLength(5)
            .MaximumLength(1024);
    }
}
