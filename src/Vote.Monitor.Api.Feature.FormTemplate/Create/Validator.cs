using Vote.Monitor.Core.Validators;
using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Api.Feature.FormTemplate.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Languages).NotEmpty();

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
