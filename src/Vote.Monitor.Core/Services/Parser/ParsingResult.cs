namespace Vote.Monitor.Core.Services.Parser;

public abstract record ParsingResult<T> where T : class
{
    public sealed record Success(List<T> Items) : ParsingResult<T>;
    public sealed record Fail(List<CsvRowParsed<T>> Items) : ParsingResult<T>
    {
        public Fail(string errorMessage) : this(new List<CsvRowParsed<T>>
        {
            new()
            {
                ErrorMessage = errorMessage,
                IsSuccess = false,
                OriginalRow = ""
            }
        })
        {
        }

        public Fail(CsvRowParsed<T> item) : this(new List<CsvRowParsed<T>> { item })
        {
        }
    }

    private ParsingResult() { }
}
