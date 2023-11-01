using CsvHelper.Configuration;

namespace Vote.Monitor.Core.Services.Csv;

public interface ICsvWriter<T>
{
    void Write(IEnumerable<T> collection, Stream stream);
    void Write<TMap>(IEnumerable<T> collection, Stream stream) where TMap : ClassMap<T>;
}
