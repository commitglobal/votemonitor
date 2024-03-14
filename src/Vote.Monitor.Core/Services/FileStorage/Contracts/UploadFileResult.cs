namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public abstract record UploadFileResult
{
    public record Ok(string Url, string Filename) : UploadFileResult;
    public record Failed : UploadFileResult;

    private UploadFileResult() { }
}
