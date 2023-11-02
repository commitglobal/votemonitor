namespace Vote.Monitor.Feature.PollingStation.Services;

public interface IPollingStationParser
{
    Task<PollingStationParsingResult> ParseAsync(Stream stream, CancellationToken cancellationToken);
}
