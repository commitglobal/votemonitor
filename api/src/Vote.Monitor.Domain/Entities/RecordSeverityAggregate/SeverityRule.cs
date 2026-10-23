using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.RecordSeverityAggregate;

/// <summary>
/// Raises a matching record's severity to <see cref="Severity"/> when it is submitted or edited. When several rules
/// match, the highest severity wins; rules never lower a severity.
/// </summary>
public class SeverityRule : AutomationRule
{
    public SeverityLevel Severity { get; private set; }

    public override AutomationRuleKind Kind => AutomationRuleKind.Severity;

    public static SeverityRule Create(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions,
        SeverityLevel severity) =>
        new(electionRoundId, monitoringNgoId, name, recordTypes, conditions, severity);

    public void Update(string name, ReportRecordType[] recordTypes, JsonDocument? conditions, SeverityLevel severity)
    {
        Update(name, recordTypes, conditions);
        Severity = severity;
    }

    private SeverityRule(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions,
        SeverityLevel severity) : base(electionRoundId, monitoringNgoId, name, recordTypes, conditions)
    {
        Severity = severity;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private SeverityRule()
    {
    }
#pragma warning restore CS8618
}
