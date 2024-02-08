namespace Vote.Monitor.Api.Feature.UserPreferences.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.LanguageIso).NotEmpty().When(x => x.LanguageId == null);
        RuleFor(x => x.LanguageId).NotEmpty().When(x => string.IsNullOrEmpty(x.LanguageIso));
    }
}
