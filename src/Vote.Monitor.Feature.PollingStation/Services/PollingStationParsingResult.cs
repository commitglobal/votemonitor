namespace Vote.Monitor.Feature.PollingStation.Services;

public abstract record PollingStationParsingResult
{
    public sealed record Success(List<PollingStationImportModel> PollingStations) : PollingStationParsingResult;
    public sealed record Fail(List<ValidationResult> ValidationErrors) : PollingStationParsingResult;

    private PollingStationParsingResult() { }
}
