using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;

public class SelectOptionRequestValidator : Validator<SelectOptionRequest>
{
    public SelectOptionRequestValidator(List<string> languages)
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Text)
            .SetValidator(new PartiallyTranslatedStringValidator(languages, 1, 256));
    }
}
