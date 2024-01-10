namespace Vote.Monitor.Api.Feature.Forms.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.LanguageId)
            .NotEmpty();

        RuleFor(x => x.Description)
            .MinimumLength(3)
            .NotEmpty();
    }
}
