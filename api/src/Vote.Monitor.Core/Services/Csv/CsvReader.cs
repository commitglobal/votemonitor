using System.Globalization;
using CsvHelper.Configuration;

namespace Vote.Monitor.Core.Services.Csv;

public class CsvReader<T> : ICsvReader<T>
{
    public IEnumerable<T> Read<TMap>(Stream stream) where TMap : ClassMap<T>
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };

        using var reader = new StreamReader(stream);
        using var csv = new CsvHelper.CsvReader(reader, config);

        csv.Context.RegisterClassMap<TMap>();

        return csv.GetRecords<T>().ToList();
    }

    public IEnumerable<T> Read(Stream stream)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower(),
        };

        using var reader = new StreamReader(stream);
        using var csv = new CsvHelper.CsvReader(reader, config);

        return csv.GetRecords<T>().ToList();
    }
}