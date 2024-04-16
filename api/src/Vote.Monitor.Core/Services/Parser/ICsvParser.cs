
namespace Vote.Monitor.Core.Services.Parser;

public interface ICsvParser<T> where T : class
{
    ParsingResult<T> Parse(Stream stream);
}
