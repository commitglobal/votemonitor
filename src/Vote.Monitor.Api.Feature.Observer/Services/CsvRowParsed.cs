namespace Vote.Monitor.Api.Feature.Observer.Services;
public sealed class CsvRowParsed<T> where T : class
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public required string OriginalRow { get; set; }
    public T? Value { get; set; }

}




