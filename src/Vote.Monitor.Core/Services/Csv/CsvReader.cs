using System.Globalization;
using CsvHelper.Configuration;

namespace Vote.Monitor.Core.Services.Csv;

public class CsvReader<T> : ICsvReader<T>
{
    public IEnumerable<T> Read<TMap>(Stream stream) where TMap : ClassMap<T>
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Context.RegisterClassMap<TMap>();

        return csv.GetRecords<T>().ToList();
    }

    public IEnumerable<T> Read(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<T>().ToList();
    }
}
