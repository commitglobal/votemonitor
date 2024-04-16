namespace Vote.Monitor.Api.Feature.ElectionRound.Unarchive;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
