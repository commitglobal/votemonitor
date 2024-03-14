using Vote.Monitor.Domain.Constants;
using Vote.Monitor.Form.Module.Requests;
using Vote.Monitor.Form.Module.Validators;

namespace Vote.Monitor.Api.Feature.PollingStation.InformationForm.Update;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Languages).NotEmpty();

        RuleForEach(x => x.Languages)
            .NotNull()
            .NotEmpty()
            .Must(iso => !string.IsNullOrWhiteSpace(iso) && LanguagesList.GetByIso(iso) != null)
            .WithMessage("Unknown language iso.");

        RuleForEach(x => x.Questions)
            .SetInheritanceValidator(v =>
            {
                v.Add<TextQuestionRequest>(r => new TextInputQuestionRequestValidator(r.Languages));
                v.Add<NumberQuestionRequest>(r => new NumberInputQuestionRequestValidator(r.Languages));
                v.Add<DateQuestionRequest>(r => new DateInputQuestionRequestValidator(r.Languages));
                v.Add<SingleSelectQuestionRequest>(r => new SingleSelectQuestionRequestValidator(r.Languages));
                v.Add<MultiSelectQuestionRequest>(r => new MultiSelectQuestionRequestValidator(r.Languages));
                v.Add<RatingQuestionRequest>(r => new RatingQuestionRequestValidator(r.Languages));
            });
    }
}
