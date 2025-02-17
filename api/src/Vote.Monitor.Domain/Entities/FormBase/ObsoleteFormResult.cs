using FluentValidation.Results;

namespace Vote.Monitor.Domain.Entities.FormBase;

public abstract record ObsoleteFormResult
{
    public record Obsoleted : ObsoleteFormResult;
    public record Error(ValidationResult Problems) : ObsoleteFormResult;

    private ObsoleteFormResult()
    {
    }
}
