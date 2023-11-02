namespace Vote.Monitor.CSOAdmin.Import;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CSOId)
            .NotEmpty();
    }
}
