using FluentValidation.Results;

namespace Vote.Monitor.Domain.Entities.FormTemplateAggregate;

public abstract record FormTemplatePublishResult
{
    public record Published : FormTemplatePublishResult;
    public record InvalidFormTemplate(ValidationResult Problems) : FormTemplatePublishResult;

    private FormTemplatePublishResult()
    {
    }
}
