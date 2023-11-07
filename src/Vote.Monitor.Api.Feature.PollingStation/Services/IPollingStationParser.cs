namespace Vote.Monitor.Api.Feature.PollingStation.Services;

public interface IPollingStationParser
{
    PollingStationParsingResult Parse(Stream stream);
}
