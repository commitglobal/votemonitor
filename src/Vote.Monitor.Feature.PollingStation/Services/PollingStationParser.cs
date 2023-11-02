namespace Vote.Monitor.Feature.PollingStation.Services;

public class PollingStationParser : IPollingStationParser
{
    private readonly PollingStationImportModelValidator _pollingStationImportModelValidator = new();
    private readonly ICsvReader<PollingStationImportModel> _reader;
    private readonly PollingStationParserConfig _parserConfig;

    public PollingStationParser(ICsvReader<PollingStationImportModel> reader, IOptions<PollingStationParserConfig> options)
    {
        _reader = reader;
        _parserConfig = options.Value;
    }

    public async Task<PollingStationParsingResult> ParseAsync(Stream stream, CancellationToken cancellationToken)
    {
        var pollingStations = await _reader
            .ReadAsync<ImportModelMapper>(stream, cancellationToken)
            .ToListAsync(cancellationToken);

        int numberOfInvalidRows = 0;
        int rowIndex = 0;
        var validationErrors = new List<ValidationResult>();

        foreach (var pollingStation in pollingStations)
        {
            var validationResult = Validate(pollingStation, rowIndex);
            if (!validationResult.IsValid)
            {
                validationErrors.Add(validationResult);
                ++numberOfInvalidRows;
                if (numberOfInvalidRows > _parserConfig.MaxParserErrorsReturned)
                {
                    break;
                }
            }

            rowIndex++;
        }

        if (validationErrors.Any())
        {
            return new PollingStationParsingResult.Fail(validationErrors);
        }

        return new PollingStationParsingResult.Success(pollingStations);
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
