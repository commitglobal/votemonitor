using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Core.Validation;
using Vote.Monitor.Domain.Entities.FormBase.Validation;

namespace Vote.Monitor.Domain.Entities.FormBase;

public class BaseFormValidator : Validator<BaseForm>
{
    public BaseFormValidator()
    {
        RuleFor(x => x.Name).SetValidator(new TranslatedStringValidator());
        RuleFor(x => x.Code).NotEmpty();

        RuleForEach(x => x.Questions)
            .SetInheritanceValidator(v =>
            {
                v.Add(new TextQuestionValidator());
                v.Add(new NumberQuestionValidator());
                v.Add(new DateQuestionValidator());
                v.Add(new SingleSelectQuestionValidator());
                v.Add(new MultiSelectQuestionValidator());
                v.Add(new RatingQuestionValidator());
            });
    }
}
