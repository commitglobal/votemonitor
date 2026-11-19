using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.FollowUpAggregate;

/// <summary>
/// Sets a matching record's follow-up status to <see cref="FollowUpStatusId"/> when it is submitted or edited, while
/// the record still has the NGO's default status. It never overrides a status a person (or an earlier rule) chose;
/// when several rules match, the first by name whose status isn't the default wins.
/// </summary>
public class FollowUpRule : AutomationRule
{
    /// <summary>One of the NGO's follow-up statuses in the round.</summary>
    public Guid FollowUpStatusId { get; private set; }

    public override AutomationRuleKind Kind => AutomationRuleKind.FollowUp;

    public static FollowUpRule Create(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions,
        Guid followUpStatusId) =>
        new(electionRoundId, monitoringNgoId, name, recordTypes, conditions, followUpStatusId);

    public void Update(string name, ReportRecordType[] recordTypes, JsonDocument? conditions, Guid followUpStatusId)
    {
        Update(name, recordTypes, conditions);
        FollowUpStatusId = followUpStatusId;
    }

    /// <summary>Its status is being deleted: the rule sets the status the status's records move to instead.</summary>
    public void MoveTo(Guid followUpStatusId)
    {
        FollowUpStatusId = followUpStatusId;
    }

    private FollowUpRule(Guid electionRoundId,
        Guid monitoringNgoId,
        string name,
        ReportRecordType[] recordTypes,
        JsonDocument? conditions,
        Guid followUpStatusId) : base(electionRoundId, monitoringNgoId, name, recordTypes, conditions)
    {
        FollowUpStatusId = followUpStatusId;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private FollowUpRule()
    {
    }
#pragma warning restore CS8618
}
