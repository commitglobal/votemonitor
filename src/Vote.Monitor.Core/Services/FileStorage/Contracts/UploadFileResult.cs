namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public abstract record UploadFileResult
{
    public record Ok(string Url, string Filename, int UrlValidityInSeconds) : UploadFileResult;
    public record Failed(string ErrorMessage) : UploadFileResult;

    private UploadFileResult() { }
}
