
namespace Vote.Monitor.Api.Feature.Observer.Services;

public interface ICsvParser<T> where T : class
{
    ParsingResult<T> Parse(Stream stream);
}
