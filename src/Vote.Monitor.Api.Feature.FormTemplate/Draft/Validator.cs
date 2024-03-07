namespace Vote.Monitor.Api.Feature.FormTemplate.Draft;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
