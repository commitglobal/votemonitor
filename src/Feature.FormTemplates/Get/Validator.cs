namespace Feature.FormTemplates.Get;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
