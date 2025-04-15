namespace Feature.Ngos.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(256)
            .NotEmpty();
    }
}
