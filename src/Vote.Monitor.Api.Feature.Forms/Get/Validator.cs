namespace Vote.Monitor.Api.Feature.Forms.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
