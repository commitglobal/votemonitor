using Vote.Monitor.Core.Validators;

namespace Feature.Forms.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.DefaultLanguage)
            .IsValidLanguageCode()
            .Must((request, iso) => request.Languages.Contains(iso))
            .WithMessage("Languages should contain declared default language.");

        RuleFor(x => x.Languages).NotEmpty();

        RuleForEach(x => x.Languages).IsValidLanguageCode();

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Name)
            .SetValidator(x => new PartiallyTranslatedStringValidator(x.Languages));

        RuleFor(x => x.Description)
            .SetValidator(x => new PartiallyTranslatedStringValidator(x.Languages));

        RuleFor(x => x.FormType)
            .NotEmpty();
    }
}
