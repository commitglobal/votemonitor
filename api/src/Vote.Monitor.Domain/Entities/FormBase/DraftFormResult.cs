using FluentValidation.Results;

namespace Vote.Monitor.Domain.Entities.FormBase;

public abstract record DraftFormResult
{
    public record Drafted : DraftFormResult;
    public record Error(ValidationResult Problems) : DraftFormResult;

    private DraftFormResult()
    {
    }
}
