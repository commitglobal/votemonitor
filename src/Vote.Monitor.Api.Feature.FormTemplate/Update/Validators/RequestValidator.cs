using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;
using Vote.Monitor.Domain.Constants;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Languages).NotEmpty();

        RuleForEach(x => x.Languages)
            .NotNull()
            .NotEmpty()
            .Must(iso => !string.IsNullOrWhiteSpace(iso) && LanguagesList.GetByIso(iso) != null)
            .WithMessage("Unknown language iso.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Name).SetValidator(x => new PartiallyTranslatedStringValidator(x.Languages, 3, 256));

        RuleFor(x => x.FormType)
            .NotEmpty();

        RuleForEach(x => x.Sections)
            .SetValidator(x => new SectionRequestValidator(x.Languages));
    }
}
