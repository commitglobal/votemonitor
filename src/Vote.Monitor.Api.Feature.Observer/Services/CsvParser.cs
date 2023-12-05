using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vote.Monitor.Api.Feature.Observer;
using Vote.Monitor.Api.Feature.Observer.Services;

namespace Vote.Monitor.Api.Feature.PollingStation.Services;



public class CsvParser<T, TMapper> : ICsvParser<T> where T : class, IDuplicateCheck where TMapper : ClassMap<T>
{
    private readonly Validator<T> _modelValidator;
    private readonly ILogger _logger;
    private readonly CsvFileConfig _parserConfig;

    public CsvParser(
        ILogger<CsvParser<T, TMapper>> logger,
        IOptions<CsvFileConfig> options,
        Validator<T> modelValidator
        )
    {

        _logger = logger;
        _parserConfig = options.Value;
        _modelValidator = modelValidator;
    }

    public ParsingResult2<T> Parse(Stream stream)
    {
        try
        {
            int numberOfInvalidRows = 0;
            int rowIndex = 1;
            var validationErrors = new List<ValidationResult>();
            List<CsvRowParsed<T>> rowsRead = new List<CsvRowParsed<T>>();

            using var reader = new StreamReader(stream);

            var readerConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ExceptionMessagesContainRawData = false,
                MissingFieldFound = null
            };



            using var csv = new CsvHelper.CsvReader(reader, readerConfiguration);
            csv.Context.RegisterClassMap<TMapper>();

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
                    ++numberOfInvalidRows;
                    currentRow.IsSuccess = false;
                    currentRow.ErrorMessage = $"Malformed row {rowIndex}";
                    validationErrors.Add(new ValidationResult(new List<ValidationFailure> { new("Csv File", $"Malformed row {rowIndex}") }));
                    continue;
                }
                var validationResult = Validate(item, rowIndex);
                if (!validationResult.IsValid)
                {
                    validationErrors.Add(validationResult);
                    ++numberOfInvalidRows;
                    currentRow.IsSuccess = false;
                    currentRow.ErrorMessage = validationResult.ToString(",");
                    if (numberOfInvalidRows >= _parserConfig.MaxParserErrorsReturned)
                    {
                        break;
                    }
                }
               
                rowIndex++;
            }

            List<ValidationFailure> duplicateserror = CsvParser<T, TMapper>.ContainsDuplicates(rowsRead);
            if (duplicateserror != null && duplicateserror.Count > 0) validationErrors.Add(new ValidationResult());

            if (validationErrors.Any())
            {
                return new ParsingResult2<T>.Fail(rowsRead, validationErrors.ToArray());
            }

            return new ParsingResult2<T>.Success(rowsRead.Select(x => x.Value));
        }
        catch (HeaderValidationException e)
        {
            _logger.LogError(e, "Cannot parse the header.");
            return new ParsingResult2<T>.Fail(new(), new ValidationFailure("Header", "Invalid header provided."));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cannot parse the file.");
            return new ParsingResult2<T>.Fail(new(), new ValidationFailure("Other error", e.Message));
        }

    }

    private static List<ValidationFailure> ContainsDuplicates(List<CsvRowParsed<T>> rows)
    {
        HashSet<string> set = new();
        List<ValidationFailure> validationFailures = new();
        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            if (row.Value != null && !set.Add(row.Value.DuplicateCheckValue))
            {
                validationFailures.Add(new ValidationFailure("DuplicateCheckValue", $"Row {i} is duplicated"));
                row.IsSuccess = false;
                row.ErrorMessage += "Row duplicated";
            }
        }

        return validationFailures;


    }

    private ValidationResult Validate(T item, int rowIndex)
    {
        var validationContext = new FluentValidation.ValidationContext<T>(item)
        {
            RootContextData =
            {
                ["RowIndex"] = rowIndex
            }
        };

        return _modelValidator.Validate(validationContext);
    }
}
