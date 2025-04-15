namespace Feature.ElectionRounds.Unstart;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
