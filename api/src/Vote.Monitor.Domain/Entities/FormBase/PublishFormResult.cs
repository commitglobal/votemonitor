using FluentValidation.Results;

namespace Vote.Monitor.Domain.Entities.FormBase;

public abstract record PublishFormResult
{
    public record Published : PublishFormResult;
    public record Error(ValidationResult Problems) : PublishFormResult;

    private PublishFormResult()
    {
    }
}
