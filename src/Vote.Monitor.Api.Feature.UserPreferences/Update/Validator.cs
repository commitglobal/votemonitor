namespace Vote.Monitor.Api.Feature.UserPreferences.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.LanguageId).NotEmpty().Must(LanguagesList.IsKnownLanguage);
    }
}
