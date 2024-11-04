using Vote.Monitor.Core.Validators;

namespace Feature.Forms.FromTemplate;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.TemplateId).NotEmpty();
        
        RuleFor(x => x.DefaultLanguage)
            .IsValidLanguageCode()
            .Must((request, iso) => request.Languages?.Contains(iso) ?? false)
            .WithMessage("Languages should contain declared default language.");

        RuleFor(x => x.Languages).NotEmpty();

        RuleForEach(x => x.Languages).IsValidLanguageCode();
    }
}
