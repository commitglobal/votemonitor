using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Feature.ElectionRounds;

public record ElectionRoundModel
{
    public required Guid Id { get; init; }
    public required Guid CountryId { get; init; }

    public required string CountryIso2 { get; init; }

    public required string CountryIso3 { get; init; }

    public required string CountryNumericCode { get; init; }

    public required string CountryName { get; init; }

    public required string CountryFullName { get; init; }

    public required string Title { get; init; }
    public required string EnglishTitle { get; init; }
    public required DateOnly StartDate { get; init; }

    public required ElectionRoundStatus Status { get; init; }

    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }

    #region ngo-admin fields

    public required bool? IsMonitoringNgoForCitizenReporting { get; init; }
    public required bool? IsCoalitionLeader { get; init; }
    public required bool? AllowMultipleFormSubmission { get; init; }

    public required Guid? CoalitionId { get; init; }
    public required string? CoalitionName { get; init; }

    #endregion

    #region platform-admin fields

    public int? NumberOfNgosMonitoring { get; init; }
    public List<MonitoringNgoModel>? MonitoringNgos { get; init; } = null;

    #endregion
}

public record MonitoringNgoModel
{
    public static MonitoringNgoModel FromEntity(Ngo ngo)
    {
        return new(ngo.Id, ngo.Name, ngo.Status);
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }
    public NgoStatus Status { get; private set; }

    [JsonConstructor]
    private MonitoringNgoModel(Guid id, string name, NgoStatus status)
    {
        Name = name;
        Status = status;
        Id = id;
    }
}
