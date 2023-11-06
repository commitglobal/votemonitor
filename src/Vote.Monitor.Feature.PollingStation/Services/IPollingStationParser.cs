namespace Vote.Monitor.Feature.PollingStation.Services;

public interface IPollingStationParser
{
    PollingStationParsingResult Parse(Stream stream);
}
