
namespace Vote.Monitor.Api.Feature.Observer.Services;

public interface ICsvParser<T> where T : class
{
    ParsingResult2<T> Parse(Stream stream);
}
