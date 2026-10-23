namespace Vote.Monitor.Domain.Entities.ReviewAggregate;

/// <summary>
/// A monitoring NGO's automation rule: it acts on the NGO's records of <see cref="RecordTypes"/> that match its
/// <see cref="Conditions"/>. Rules only act on future events; saving one never changes existing records.
/// </summary>
public abstract class AutomationRule : AuditableBaseEntity, IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public string Name { get; private set; }
    public ReportRecordType[] RecordTypes { get; private set; }

    /// <summary>An advanced filter (the V2 lists' format) the record must match; null matches every record.</summary>
    public JsonDocument? Conditions { get; private set; }

    public abstract AutomationRuleKind Kind { get; }

    protected AutomationRule(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions)
    {
        Id = Guid.NewGuid();
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        Name = name;
        RecordTypes = recordTypes;
        Conditions = conditions;
    }

    protected void Update(string name, ReportRecordType[] recordTypes, JsonDocument? conditions)
    {
        Name = name;
        RecordTypes = recordTypes;
        Conditions = conditions;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    protected AutomationRule()
    {
    }
#pragma warning restore CS8618
}
