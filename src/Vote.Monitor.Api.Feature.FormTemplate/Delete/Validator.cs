namespace Vote.Monitor.Api.Feature.FormTemplate.Delete;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
