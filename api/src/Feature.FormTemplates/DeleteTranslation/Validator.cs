using Vote.Monitor.Core.Validators;

namespace Feature.FormTemplates.DeleteTranslation;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.LanguageCode)
            .IsValidLanguageCode();
    }
}
