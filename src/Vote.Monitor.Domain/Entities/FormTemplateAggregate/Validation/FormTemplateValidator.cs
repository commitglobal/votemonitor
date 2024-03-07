using FastEndpoints;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate.Validation;

public class FormTemplateValidator : Validator<FormTemplate>
{
    public FormTemplateValidator()
    {
        RuleFor(x => x.Name).SetValidator(new TranslatedStringValidator());

        RuleForEach(x => x.Sections)
            .SetValidator(x => new SectionValidator());
    }
}
