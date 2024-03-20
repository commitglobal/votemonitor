using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Form.Module.Validators;

namespace Feature.PollingStation.Information.Form.Upsert;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();

        RuleFor(x => x.Languages).NotEmpty();

        RuleForEach(x => x.Languages)
            .NotNull()
            .NotEmpty()
            .Must(iso => !string.IsNullOrWhiteSpace(iso) && LanguagesList.GetByIso(iso) != null)
            .WithMessage("Unknown language iso.");

        RuleForEach(x => x.Questions)
            .SetInheritanceValidator(v =>
            {
                v.Add(r => new TextQuestionRequestValidator(r.Languages));
                v.Add(r => new NumberQuestionRequestValidator(r.Languages));
                v.Add(r => new DateQuestionRequestValidator(r.Languages));
                v.Add(r => new SingleSelectQuestionRequestValidator(r.Languages));
                v.Add(r => new MultiSelectQuestionRequestValidator(r.Languages));
                v.Add(r => new RatingQuestionRequestValidator(r.Languages));
            });
    }
}
