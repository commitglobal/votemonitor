using Vote.Monitor.Core.Validators;

namespace Feature.FormTemplates.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Languages).NotEmpty();

        RuleFor(x => x.DefaultLanguage)
            .IsValidLanguageCode()
            .Must((request, iso) => request.Languages?.Contains(iso) ?? false)
            .WithMessage("Languages should contain declared default language.");

        RuleForEach(x => x.Languages)
            .IsValidLanguageCode();

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Name)
            .SetValidator(x => new PartiallyTranslatedStringValidator(x.Languages, 3, 256));

        RuleFor(x => x.Description)
            .SetValidator(x => new PartiallyTranslatedStringValidator(x.Languages, 3, 256));

        RuleFor(x => x.FormTemplateType)
            .NotEmpty();
    }
}
