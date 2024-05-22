using System.Globalization;
using CsvHelper.Configuration;

namespace Vote.Monitor.Core.Services.Csv;

public class CsvWriter : ICsvWriter
{
    public void Write<T>(IEnumerable<T> collection, Stream stream)
    {
        using var writer = new StreamWriter(stream);
        using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteRecords(collection);
    }

    public void Write<T, TMap>(IEnumerable<T> collection, Stream stream) where TMap : ClassMap<T>
    {
        using var writer = new StreamWriter(stream);
        using var csv = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<TMap>();

        csv.WriteRecordsAsync(collection);
    }
}
