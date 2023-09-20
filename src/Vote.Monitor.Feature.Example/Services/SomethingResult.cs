namespace Vote.Monitor.Feature.Example.Services;

public abstract record SomethingResult
{
    public record Ok (string Result): SomethingResult;
    public record Error (params string[] Errors): SomethingResult;

    private SomethingResult()
    {
    }
}
