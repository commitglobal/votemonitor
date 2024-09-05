namespace Vote.Monitor.Api.Feature.ElectionRound;

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

    [JsonConverter(typeof(SmartEnumNameConverter<ElectionRoundStatus, string>))]
    public required ElectionRoundStatus Status { get; init; }

    public required DateTime CreatedOn { get; init; }
    public required DateTime? LastModifiedOn { get; init; }
}