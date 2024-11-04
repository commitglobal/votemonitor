using Vote.Monitor.Core.Validators;
using Vote.Monitor.Form.Module.Validators;

namespace Feature.Forms.FromForm;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.FormElectionRoundId).NotEmpty();
        RuleFor(x => x.FormId).NotEmpty();
        RuleFor(x => x.DefaultLanguage)
            .IsValidLanguageCode()
            .Must((request, iso) => request.Languages?.Contains(iso) ?? false)
            .WithMessage("Languages should contain declared default language.");

        RuleFor(x => x.Languages).NotEmpty();

        RuleForEach(x => x.Languages).IsValidLanguageCode();
    }
}
