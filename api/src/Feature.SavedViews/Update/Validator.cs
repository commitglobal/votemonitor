namespace Feature.SavedViews.Update;

public class Validator : SaveViewRequestValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ViewId).NotEmpty();
    }
}
