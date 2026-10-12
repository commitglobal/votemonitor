namespace Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;

/// <summary>
/// One move of a form submission, quick report or incident report to another polling station by a platform admin.
/// </summary>
public class PollingStationCorrection : IAggregateRoot
{
    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public ElectionRound ElectionRound { get; private set; }
    public CorrectableRecordType RecordType { get; private set; }
    public Guid RecordId { get; private set; }
    public Guid FromPollingStationId { get; private set; }
    public PollingStation FromPollingStation { get; private set; }
    public Guid ToPollingStationId { get; private set; }
    public PollingStation ToPollingStation { get; private set; }
    public DateTime CorrectedAt { get; private set; }

    /// <summary>The platform admin who made the move. Stored, never returned.</summary>
    public Guid CorrectedBy { get; private set; }

    public static PollingStationCorrection Create(Guid electionRoundId,
        CorrectableRecordType recordType,
        Guid recordId,
        Guid fromPollingStationId,
        Guid toPollingStationId,
        DateTime correctedAt,
        Guid correctedBy) =>
        new(Guid.NewGuid(), electionRoundId, recordType, recordId, fromPollingStationId, toPollingStationId,
            correctedAt, correctedBy);

    private PollingStationCorrection(Guid id,
        Guid electionRoundId,
        CorrectableRecordType recordType,
        Guid recordId,
        Guid fromPollingStationId,
        Guid toPollingStationId,
        DateTime correctedAt,
        Guid correctedBy)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        RecordType = recordType;
        RecordId = recordId;
        FromPollingStationId = fromPollingStationId;
        ToPollingStationId = toPollingStationId;
        CorrectedAt = correctedAt;
        CorrectedBy = correctedBy;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private PollingStationCorrection()
    {
    }
#pragma warning restore CS8618
}
