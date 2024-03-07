using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Validation;

public class SectionValidator : Validator<FormSection>
{
    public SectionValidator()
    {
        RuleFor(x => x.Title)
            .SetValidator(new TranslatedStringValidator());

        RuleForEach(x => x.Questions)
            .SetInheritanceValidator(v =>
            {
                v.Add<TextInputQuestion>(new TextInputQuestionValidator());
                v.Add<NumberInputQuestion>(new NumberInputQuestionValidator());
                v.Add<DateInputQuestion>(new DateInputQuestionValidator());
                v.Add<SingleSelectQuestion>(new SingleSelectQuestionValidator());
                v.Add<MultiSelectQuestion>(new MultiSelectQuestionValidator());
                v.Add<RatingQuestion>(new RatingQuestionValidator());
            });
    }
}