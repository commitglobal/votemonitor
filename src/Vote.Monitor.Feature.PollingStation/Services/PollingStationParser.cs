namespace Vote.Monitor.Feature.PollingStation.Services;

public class PollingStationParser : IPollingStationParser
{
    private readonly PollingStationImportModelValidator _pollingStationImportModelValidator = new();
    private readonly ICsvReader<PollingStationImportModel> _reader;
    private readonly ILogger<PollingStationParser> _logger;
    private readonly PollingStationParserConfig _parserConfig;

    public PollingStationParser(ICsvReader<PollingStationImportModel> reader,
        ILogger<PollingStationParser> logger,
        IOptions<PollingStationParserConfig> options)
    {
        _reader = reader;
        _logger = logger;
        _parserConfig = options.Value;
    }

    public PollingStationParsingResult Parse(Stream stream)
    {
        try
        {
            var pollingStations = _reader
                .Read<PollingStationImportModelMapper>(stream)
                .ToList();

            int numberOfInvalidRows = 0;
            int rowIndex = 1;
            var validationErrors = new List<ValidationResult>();

            foreach (var pollingStation in pollingStations)
            {
                var validationResult = Validate(pollingStation, rowIndex);
                if (!validationResult.IsValid)
                {
                    validationErrors.Add(validationResult);
                    ++numberOfInvalidRows;
                    if (numberOfInvalidRows >= _parserConfig.MaxParserErrorsReturned)
                    {
                        break;
                    }
                }

                rowIndex++;
            }

            if (validationErrors.Any())
            {
                return new PollingStationParsingResult.Fail(validationErrors.ToArray());
            }

            return new PollingStationParsingResult.Success(pollingStations);
        }
        catch (HeaderValidationException e)
        {
            _logger.LogError(e, "Cannot parse the header.");
            return new PollingStationParsingResult.Fail(new ValidationFailure("Header", "Invalid header provided in import polling stations file."));
        }
        catch (CsvHelper.MissingFieldException e)
        {
            _logger.LogError(e, "Malformed csv provided.");
            return new PollingStationParsingResult.Fail(new ValidationFailure("Csv File", "Malformed import polling stations file provided."));
        }
        catch (CsvHelper.TypeConversion.TypeConverterException e)
        {
            _logger.LogError(e, "Invalid data found in columns.");
            return new PollingStationParsingResult.Fail(new ValidationFailure("Csv File", "Malformed import polling stations file provided."));
        }
    }

    private ValidationResult Validate(PollingStationImportModel pollingStation, int rowIndex)
    {
        var validationContext = new FluentValidation.ValidationContext<PollingStationImportModel>(pollingStation)
        {
            RootContextData =
            {
                ["RowIndex"] = rowIndex
            }
        };

        return _pollingStationImportModelValidator.Validate(validationContext);
    }
}
