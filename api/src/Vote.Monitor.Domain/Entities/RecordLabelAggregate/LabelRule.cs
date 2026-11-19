using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.RecordLabelAggregate;

/// <summary>
/// Adds <see cref="Labels"/> to a matching record when it is submitted or edited. It only ever adds labels, never
/// removes one; a label the record already carries (ignoring case) is left as is.
/// </summary>
public class LabelRule : AutomationRule
{
    /// <summary>The most labels one rule can add.</summary>
    public const int MaxLabels = 10;

    /// <summary>The labels to add: trimmed, distinct ignoring case.</summary>
    public string[] Labels { get; private set; }

    public override AutomationRuleKind Kind => AutomationRuleKind.Labels;

    public static LabelRule Create(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions,
        string[] labels) =>
        new(electionRoundId, monitoringNgoId, name, recordTypes, conditions, labels);

    public void Update(string name, ReportRecordType[] recordTypes, JsonDocument? conditions, string[] labels)
    {
        Update(name, recordTypes, conditions);
        Labels = labels;
    }

    private LabelRule(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions,
        string[] labels) : base(electionRoundId, monitoringNgoId, name, recordTypes, conditions)
    {
        Labels = labels;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private LabelRule()
    {
    }
#pragma warning restore CS8618
}
