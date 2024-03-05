using Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;
using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Api.Feature.FormTemplate.Create;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleForEach(x => x.Languages)
            .NotEmpty()
            .Must(iso => LanguagesList.GetByIso(iso) != null)
            .WithMessage("Unknown language iso.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Name)
            .SetValidator(x => new PartiallyTranslatedStringValidator(x.Languages, 3, 256));

        RuleFor(x => x.FormType)
            .NotEmpty();
    }
}
