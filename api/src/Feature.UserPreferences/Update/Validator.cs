using Vote.Monitor.Core.Constants;

namespace Feature.UserPreferences.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.LanguageCode)
            .NotEmpty()
            .Must(LanguagesList.IsKnownLanguage)
            .WithMessage("Unknown language id.");
    }
}
