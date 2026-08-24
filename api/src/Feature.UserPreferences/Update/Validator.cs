namespace Feature.UserPreferences.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Preferences).NotNull();
    }
}
