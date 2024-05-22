using CsvHelper.Configuration;

namespace Vote.Monitor.Core.Services.Csv;

public interface ICsvWriter
{
    void Write<T>(IEnumerable<T> collection, Stream stream);
    void Write<T, TMap>(IEnumerable<T> collection, Stream stream) where TMap : ClassMap<T>;
}
