using Vote.Monitor.Domain.Entities.ReviewAggregate;

namespace Vote.Monitor.Domain.Entities.SavedViewAggregate;

/// <summary>
/// A named state of one of a monitoring NGO's report lists in a round (filters, query, search, sort, display), saved by
/// one of its members. The web app owns <see cref="State"/>; the API stores it as given and never reads it. The member
/// who saved it is <see cref="AuditableBaseEntity.CreatedBy"/>; <see cref="CreatedByName"/> keeps their name for shared
/// views that outlive their account.
/// </summary>
public class SavedView : AuditableBaseEntity, IAggregateRoot
{
    public const int MaxNameLength = 100;
    public const int MaxDescriptionLength = 500;
    public const int MaxCreatorNameLength = 256;

    /// <summary>The most bytes a view's state can take once serialised.</summary>
    public const int MaxStateBytes = 32 * 1024;

    /// <summary>The most views an NGO can have per round and record type, shared and private together.</summary>
    public const int MaxViewsPerRecordType = 200;

    public Guid Id { get; private set; }
    public Guid ElectionRoundId { get; private set; }
    public Guid MonitoringNgoId { get; private set; }
    public ReportRecordType RecordType { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public SavedViewAccess Access { get; private set; }
    public JsonDocument State { get; private set; }
    public string CreatedByName { get; private set; }

    public static SavedView Create(Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        string name,
        string? description,
        SavedViewAccess access,
        JsonDocument state,
        Guid createdBy,
        string createdByName) =>
        new(Guid.NewGuid(), electionRoundId, monitoringNgoId, recordType, name, description, access, state, createdBy,
            createdByName);

    public void Update(string name, string? description, SavedViewAccess access, JsonDocument state)
    {
        Name = name;
        Description = description;
        Access = access;
        State = state;
    }

    private SavedView(Guid id,
        Guid electionRoundId,
        Guid monitoringNgoId,
        ReportRecordType recordType,
        string name,
        string? description,
        SavedViewAccess access,
        JsonDocument state,
        Guid createdBy,
        string createdByName)
    {
        Id = id;
        ElectionRoundId = electionRoundId;
        MonitoringNgoId = monitoringNgoId;
        RecordType = recordType;
        Name = name;
        Description = description;
        Access = access;
        State = state;
        CreatedBy = createdBy;
        CreatedByName = createdByName;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    private SavedView()
    {
    }
#pragma warning restore CS8618
}
