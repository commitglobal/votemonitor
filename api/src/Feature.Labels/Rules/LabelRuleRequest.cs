using Feature.Labels.Services;
using Feature.Review.Rules;

namespace Feature.Labels.Rules;

/// <summary>A label rule, as sent when creating or updating one.</summary>
public class LabelRuleRequest : SaveRuleRequest
{
    /// <summary>"Add labels": trimmed, each kept once ignoring case (its first spelling).</summary>
    public string[] Labels { get; set; } = [];
}

public abstract class LabelRuleRequestValidator<T> : SaveRuleRequestValidator<T> where T : LabelRuleRequest
{
    protected LabelRuleRequestValidator() : base(AutomationRuleKind.Labels)
    {
        RuleFor(x => x.Labels)
            .NotEmpty()
            .Must(x => x is null || x.All(label => !string.IsNullOrWhiteSpace(label)))
            .WithMessage("Labels can't be blank")
            .Must(x => x is null || x.All(label => label is null || label.Trim().Length <= RecordLabel.MaxLength))
            .WithMessage($"A label can be at most {RecordLabel.MaxLength} characters long")
            .Must(x => x is null || LabelService.Normalize(x).Count <= LabelRule.MaxLabels)
            .WithMessage($"A rule can add at most {LabelRule.MaxLabels} labels");
    }
}
