namespace Vote.Monitor.Api.Feature.PollingStation.Services;

public abstract record PollingStationParsingResult
{
    public sealed record Success(List<PollingStationImportModel> PollingStations) : PollingStationParsingResult;
    public sealed record Fail(params ValidationResult[] ValidationErrors) : PollingStationParsingResult
    {
        public Fail(ValidationFailure validationFailure) : this(new ValidationResult(new[] { validationFailure }))
        {
        }

        public void Deconstruct(out ValidationResult[] validationErrors)
        {
            validationErrors = ValidationErrors;
        }
    }

    private PollingStationParsingResult() { }
}
