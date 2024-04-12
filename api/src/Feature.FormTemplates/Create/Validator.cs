using Vote.Monitor.Core.Validators;
using Vote.Monitor.Domain.Constants;

namespace Feature.FormTemplates.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Languages).NotEmpty();

        RuleFor(x => x.DefaultLanguage)
            .NotNull()
            .NotEmpty()
            .Must(iso => !string.IsNullOrWhiteSpace(iso) && LanguagesList.GetByIso(iso) != null)
            .WithMessage("Unknown language iso.")
            .Must((request, iso) => request.Languages?.Contains(iso) ?? false)
            .WithMessage("Languages should contain declared default language.");

        RuleForEach(x => x.Languages)
            .NotEmpty()
            .Must(iso => !string.IsNullOrWhiteSpace(iso) && LanguagesList.GetByIso(iso) != null)
            .WithMessage("Unknown language iso.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Name)
            .SetValidator(x => new PartiallyTranslatedStringValidator(x.Languages, 3, 256));

        RuleFor(x => x.FormTemplateType)
            .NotEmpty();
    }
}
