namespace Feature.SavedViews.Create;

public class Validator : SaveViewRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.RecordType).NotNull().WithMessage("The record type is required");
    }
}
