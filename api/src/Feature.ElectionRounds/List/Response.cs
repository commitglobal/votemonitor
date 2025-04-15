namespace Feature.ElectionRounds.List;

public record Response
{
    public Guid Id { get; init; }
    public Guid CountryId { get; init; }

    public string CountryIso2 { get; init; } = string.Empty;

    public string CountryIso3 { get; init; } = string.Empty;

    public string CountryNumericCode { get; init; } = string.Empty;

    public string CountryName { get; init; } = string.Empty;

    public string CountryFullName { get; init; } = string.Empty;

    public string Title { get; init; } = string.Empty;
    public string EnglishTitle { get; init; } = string.Empty;
    public DateOnly StartDate { get; init; }

    public ElectionRoundStatus Status { get; init; } = ElectionRoundStatus.NotStarted;

    public DateTime CreatedOn { get; init; }
    public DateTime? LastModifiedOn { get; init; }

    public MonitoringNgoModel[] MonitoringNgos { get; init; } = [];
}
