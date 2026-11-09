using Feature.Review;

namespace Feature.Labels.Set;

public class Validator : ReportRequestValidator<Request>
{
    /// <summary>The most labels one record can carry.</summary>
    public const int MaxLabels = 50;

    public Validator()
    {
        RuleFor(x => x.Labels)
            .NotNull()
            .Must(labels => labels is null || labels.Length <= MaxLabels)
            .WithMessage($"A record can carry at most {MaxLabels} labels.");
        RuleForEach(x => x.Labels)
            .NotNull()
            .Must(label => label is null || label.Trim().Length <= RecordLabel.MaxLength)
            .WithMessage($"A label can be at most {RecordLabel.MaxLength} characters long.");
    }
}
