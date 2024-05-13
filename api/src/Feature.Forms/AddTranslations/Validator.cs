using Vote.Monitor.Core.Validators;

namespace Feature.Forms.AddTranslations;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.NgoId)
            .NotEmpty();

        RuleFor(x => x.LanguageCodes).NotEmpty();
        RuleForEach(x => x.LanguageCodes).IsValidLanguageCode();
    }
}
