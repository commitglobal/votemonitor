using FastEndpoints;
using Vote.Monitor.Core.Validation;
using Vote.Monitor.Domain.Entities.FormBase.Validation;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public class FormTemplateValidator : Validator<FormTemplate>
{
    public FormTemplateValidator()
    {
        RuleFor(x => x.Name).SetValidator(new TranslatedStringValidator());

        RuleForEach(x => x.Sections)
            .SetValidator(x => new SectionValidator());
    }
}
