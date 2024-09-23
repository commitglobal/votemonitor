namespace Feature.Locations.Services;

public abstract record LocationParsingResult
{
    public sealed record Success(List<LocationImportModel> Locations) : LocationParsingResult;
    public sealed record Fail(params ValidationResult[] ValidationErrors) : LocationParsingResult
    {
        public Fail(ValidationFailure validationFailure) : this(new ValidationResult(new[] { validationFailure }))
        {
        }

        public void Deconstruct(out ValidationResult[] validationErrors)
        {
            validationErrors = ValidationErrors;
        }
    }

    private LocationParsingResult() { }
}
