using FluentValidation.Results;

namespace Vote.Monitor.Domain.Entities.FormAggregate;

public abstract record FormPublishResult
{
    public record Published : FormPublishResult;
    public record InvalidForm(ValidationResult Problems) : FormPublishResult;

    private FormPublishResult()
    {
    }
}
