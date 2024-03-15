using Vote.Monitor.Core.Validators;
using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Api.Feature.Form.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.MonitoringNgoId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Languages).NotEmpty();

        RuleForEach(x => x.Languages)
            .NotNull()
            .NotEmpty()
            .Must(iso => !string.IsNullOrWhiteSpace(iso) && LanguagesList.GetByIso(iso) != null)
            .WithMessage("Unknown language iso.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Name)
            .SetValidator(x => new PartiallyTranslatedStringValidator(x.Languages, 3, 256));

        RuleFor(x => x.FormType)
            .NotEmpty();

        RuleForEach(x => x.Questions)
            .SetInheritanceValidator(v =>
            {
                v.Add<TextQuestionRequest>(x => new TextInputQuestionRequestValidator(x.Languages));
                v.Add<NumberQuestionRequest>(x => new NumberInputQuestionRequestValidator(x.Languages));
                v.Add<DateQuestionRequest>(x => new DateInputQuestionRequestValidator(x.Languages));
                v.Add<SingleSelectQuestionRequest>(x => new SingleSelectQuestionRequestValidator(x.Languages));
                v.Add<MultiSelectQuestionRequest>(x => new MultiSelectQuestionRequestValidator(x.Languages));
                v.Add<RatingQuestionRequest>(x => new RatingQuestionRequestValidator(x.Languages));
            });

        RuleForEach(x => x.Sections)
            .SetValidator((req, section) => new SectionUniquenessRequestValidator(req.Sections.Except([section])));
    }
}
