using FastEndpoints;
using FluentValidation;
using Module.Forms.Requests;
using Vote.Monitor.Core.Validators;

namespace Module.Forms.Validators;

public class SelectOptionRequestValidator : Validator<SelectOptionRequest>
{
    public SelectOptionRequestValidator(List<string> languages)
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Text)
            .SetValidator(new PartiallyTranslatedStringValidator(languages));
    }
}
