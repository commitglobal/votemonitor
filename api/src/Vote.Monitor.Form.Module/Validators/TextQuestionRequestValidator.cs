using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Core.Validators;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Form.Module.Validators;

public class TextQuestionRequestValidator : Validator<TextQuestionRequest>
{
    public TextQuestionRequestValidator(List<string> languages)
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.QuestionType).NotEmpty();

        RuleFor(x => x.Text)
            .SetValidator(new PartiallyTranslatedStringValidator(languages));

        RuleFor(x => x.Helptext)
            .SetValidator(new PartiallyTranslatedStringValidator(languages))
            .When(x => x.Helptext != null);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.InputPlaceholder)
            .SetValidator(new PartiallyTranslatedStringValidator(languages))
            .When(x => x.InputPlaceholder != null);

        RuleFor(x => x.DisplayLogic)
            .SetValidator(new DisplayLogicRequestValidator());
    }
}
