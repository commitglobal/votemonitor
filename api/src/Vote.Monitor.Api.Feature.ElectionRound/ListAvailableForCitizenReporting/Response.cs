namespace Vote.Monitor.Api.Feature.ElectionRound.ListAvailableForCitizenReporting;

public record Response
{
    public List<ElectionRoundModel> ElectionRounds { get; init; } = [];
}

public record ElectionRoundModel
{
    public Guid Id { get; init; }
    public string CountryCode { get; init; }
    public string CountryName { get; init; }
    public string CountryFullName { get; init; }
    public DateOnly StartDate { get; init; }
    public string Title { get; init; }
}