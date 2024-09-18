using Feature.Locations.Options;

namespace Feature.Locations.Services;

public class LocationParser : ILocationsParser
{
    private readonly LocationImportModelValidator _locationImportModelValidator = new();
    private readonly ICsvReader<LocationImportModel> _reader;
    private readonly ILogger<LocationParser> _logger;
    private readonly LocationParserConfig _parserConfig;

    public LocationParser(ICsvReader<LocationImportModel> reader,
        ILogger<LocationParser> logger,
        IOptions<LocationParserConfig> options)
    {
        _reader = reader;
        _logger = logger;
        _parserConfig = options.Value;
    }

    public LocationParsingResult Parse(Stream stream)
    {
        try
        {
            var locations = _reader
                .Read<LocationImportModelMapper>(stream)
                .ToList();

            int numberOfInvalidRows = 0;
            int rowIndex = 1;
            var validationErrors = new List<ValidationResult>();

            foreach (var location in locations)
            {
                var validationResult = Validate(location, rowIndex);
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
                return new LocationParsingResult.Fail(validationErrors.ToArray());
            }

            return new LocationParsingResult.Success(locations);
        }
        catch (HeaderValidationException e)
        {
            SentrySdk.CaptureException(e);
            _logger.LogError(e, "Cannot parse the header.");
            return new LocationParsingResult.Fail(new ValidationFailure("Header",
                "Invalid header provided in import locations file."));
        }
        catch (CsvHelper.MissingFieldException e)
        {
            SentrySdk.CaptureException(e);
            _logger.LogError(e, "Malformed csv provided.");
            return new LocationParsingResult.Fail(new ValidationFailure("Csv File",
                "Malformed import locations file provided."));
        }
        catch (CsvHelper.TypeConversion.TypeConverterException e)
        {
            SentrySdk.CaptureException(e);
            _logger.LogError(e, "Invalid data found in columns.");
            return new LocationParsingResult.Fail(new ValidationFailure("Csv File",
                "Malformed import locations file provided."));
        }
    }

    private ValidationResult Validate(LocationImportModel location, int rowIndex)
    {
        var validationContext = new FluentValidation.ValidationContext<LocationImportModel>(location)
        {
            RootContextData =
            {
                ["RowIndex"] = rowIndex
            }
        };

        return _locationImportModelValidator.Validate(validationContext);
    }
}