namespace Feature.PollingStations.Services;

public interface IPollingStationParser
{
    PollingStationParsingResult Parse(Stream stream);
}
