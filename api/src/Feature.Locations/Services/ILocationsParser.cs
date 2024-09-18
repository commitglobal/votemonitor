namespace Feature.Locations.Services;

public interface ILocationsParser
{
    LocationParsingResult Parse(Stream stream);
}
