using Vote.Monitor.Core.RulesEngine.Rules;

namespace Feature.Review.Rules;

/// <summary>Route and claims shared by the automation rule endpoints: the caller's NGO's rules in a round.</summary>
public class RulesRequest
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }
}

/// <summary>One of the caller's NGO's automation rules.</summary>
public class RuleRequest : RulesRequest
{
    public Guid RuleId { get; set; }
}

public class RuleRequestValidator<T> : Validator<T> where T : RuleRequest
{
    public RuleRequestValidator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.RuleId).NotEmpty();
    }
}

/// <summary>The fields every kind of automation rule has, as sent when creating or updating one.</summary>
public class SaveRuleRequest : RulesRequest
{
    public string Name { get; set; } = "";
    public ReportRecordType[] RecordTypes { get; set; } = [];

    /// <summary>The advanced filter a record must match (the V2 lists' format); null for every record.</summary>
    public FilterRule? Conditions { get; set; }
}

public abstract class SaveRuleRequestValidator<T> : Validator<T> where T : SaveRuleRequest
{
    public const int MaxNameLength = 256;

    protected SaveRuleRequestValidator(AutomationRuleKind kind)
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty()
            .Must(x => x.Trim().Length <= MaxNameLength)
            .WithMessage($"The name can have at most {MaxNameLength} characters");
        RuleFor(x => x.RecordTypes)
            .NotEmpty()
            .Must(x => x.All(recordType => recordType is not null) && x.Distinct().Count() == x.Length)
            .WithMessage("Record types must be distinct");
        RuleFor(x => x.Conditions)
            .Custom((conditions, context) =>
            {
                var error = RuleConditions.FindError(conditions, kind, context.InstanceToValidate.RecordTypes);
                if (error is not null)
                {
                    context.AddFailure(error);
                }
            })
            .When(x => x.RecordTypes.Length > 0 && x.RecordTypes.All(recordType => recordType is not null));
    }
}
