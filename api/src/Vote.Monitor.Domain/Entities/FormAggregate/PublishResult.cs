using FluentValidation.Results;

namespace Vote.Monitor.Domain.Entities.FormAggregate;

public abstract record PublishResult
{
    public record Published : PublishResult;
    public record InvalidForm(ValidationResult Problems) : PublishResult;

    private PublishResult()
    {
    }
}
