using Vote.Monitor.Core.Validators;
using Module.Forms.Validators;

namespace Feature.PollingStation.Information.Forms.Upsert;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.Languages).NotEmpty();
        RuleFor(x => x.DefaultLanguage)
            .IsValidLanguageCode()
            .Must((request, iso) => request.Languages.Contains(iso))
            .WithMessage("Languages should contain declared default language.");


        RuleForEach(x => x.Languages)
            .IsValidLanguageCode();

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
