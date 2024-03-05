namespace Vote.Monitor.Api.Feature.FormTemplate.Publish;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
