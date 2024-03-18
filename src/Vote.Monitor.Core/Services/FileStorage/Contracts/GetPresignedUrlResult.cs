namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public abstract record GetPresignedUrlResult
{
    public record Ok(string Url, string Filename) : GetPresignedUrlResult;
    public record NotFound : GetPresignedUrlResult;
    public record Failed(string ErrorMessage) : GetPresignedUrlResult;
    private GetPresignedUrlResult() { }
}
