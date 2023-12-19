using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Vote.Monitor.Api.Feature.Observer.Services;

namespace Vote.Monitor.Api.Feature.PollingStation.Services;



public class CsvParser<T, TMapper> : ICsvParser<T> where T : class where TMapper : ClassMap<T>
{
    private readonly Validator<T> _modelValidator;
    private readonly ILogger _logger;

    public CsvParser(
        ILogger<CsvParser<T, TMapper>> logger,
        Validator<T> modelValidator
        )
    {
        _logger = logger;
        _modelValidator = modelValidator;
    }

    public ParsingResult<T> Parse(Stream stream)
    {
        List<CsvRowParsed<T>> rowsRead = new List<CsvRowParsed<T>>();
        try
        {
            int rowIndex = 1;
            bool anyError = false;

            using var reader = new StreamReader(stream);

            var readerConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ExceptionMessagesContainRawData = false,
                MissingFieldFound = null
            };

            using var csv = new CsvHelper.CsvReader(reader, readerConfiguration);
            csv.Context.RegisterClassMap<TMapper>();
            csv.Read();
            csv.ReadHeader();
            csv.ValidateHeader<T>();
            rowsRead.Add(new CsvRowParsed<T>()
            {
                IsSuccess = true,
                OriginalRow = string.Join(",", csv.HeaderRecord!),
                Value = null
            });
            while (csv.Read())
            {
                T? item = csv.GetRecord<T>();
                var currentRow = new CsvRowParsed<T>()
                {
                    IsSuccess = true,
                    OriginalRow = csv.Parser.RawRecord,
                    Value = item
                };
                rowsRead.Add(currentRow);


                if (item == null)
                {
                    anyError = true;
                    currentRow.IsSuccess = false;
                    currentRow.ErrorMessage = $"Malformed row {rowIndex}";
                    continue;
                }
                var validationResult = _modelValidator.Validate(item);
                if (!validationResult.IsValid)
                {
                    anyError = true;
                    currentRow.IsSuccess = false;
                    currentRow.ErrorMessage = validationResult.ToString(",");
                }

                rowIndex++;
            }


            if (anyError || ContainsDuplicates(rowsRead))
            {
                return new ParsingResult<T>.Fail(rowsRead);
            }
            return new ParsingResult<T>.Success(rowsRead.Where(x => x.Value != null).Select(x => x.Value!));
        }
        catch (ReaderException ex)
        {
            _logger.LogError(ex, "Cannot parse the file or file empty.");
            return new ParsingResult<T>.Fail(new CsvRowParsed<T>()
            {
                IsSuccess = false,
                ErrorMessage = "Cannot parse the file or file empty.",
                OriginalRow = string.Empty
            });
        }
        catch (HeaderValidationException ex)
        {
            _logger.LogError(ex, "Cannot parse the header!");
            return new ParsingResult<T>.Fail(new CsvRowParsed<T>()
            {
                IsSuccess = false,
                ErrorMessage = "Cannot parse the header!",
                OriginalRow = string.Empty
            });
        }


    }

    private static bool ContainsDuplicates(List<CsvRowParsed<T>> rows)
    {
        Dictionary<int, int> set = new();
        bool containsDuplicates = false;
        for (int i = 1; i < rows.Count; i++)
        {
            var row = rows[i];
            if (row.IsSuccess && row.Value == null) continue;
            if (set.ContainsKey(row.Value!.GetHashCode()))
            {
                int firstRowIndex = set[row.Value.GetHashCode()];
                containsDuplicates = true;
                row.IsSuccess = false;
                row.ErrorMessage += $"Duplicated email found. First row where you can find the duplicate email is {firstRowIndex}";
            }
            else
            {
                set.Add(row.Value!.GetHashCode(), i);
            }
        }

        return containsDuplicates;
    }


}
