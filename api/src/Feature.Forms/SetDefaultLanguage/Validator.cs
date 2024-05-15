using Vote.Monitor.Core.Validators;

namespace Feature.Forms.SetDefaultLanguage;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId)
            .NotEmpty();
        RuleFor(x => x.NgoId)
            .NotEmpty();
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.LanguageCode).IsValidLanguageCode();
    }
}
