namespace Vote.Monitor.Api.Feature.Observer.Services;




public abstract record ParsingResult2<T> where T : class, IDuplicateCheck
{
    public sealed record Success(IEnumerable<T> Items) : ParsingResult2<T>;
    public sealed record Fail(IEnumerable<CsvRowParsed<T>> Items, params ValidationResult[] ValidationErrors) : ParsingResult2<T>
    {
        public Fail(CsvRowParsed<T> item, ValidationFailure validationFailure) : this(new List<CsvRowParsed<T>>() { item }, new ValidationResult(new[] { validationFailure }))
        {
        }

        public Fail(ValidationFailure validationFailure) : this(new List<CsvRowParsed<T>>(), new ValidationResult(new[] { validationFailure }))
        {
        }

        public void Deconstruct(out ValidationResult[] validationErrors)
        {
            validationErrors = ValidationErrors;
        }
    }


    private ParsingResult2() { }
}
