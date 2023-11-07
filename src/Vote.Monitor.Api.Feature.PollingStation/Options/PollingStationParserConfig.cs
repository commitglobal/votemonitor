namespace Vote.Monitor.Api.Feature.PollingStation.Options;

public class PollingStationParserConfig
{
    public const string Key = "ParserConfig";
    public int MaxParserErrorsReturned { get; set; }
}
