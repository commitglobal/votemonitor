namespace Vote.Monitor.Core.Services.FileStorage.Contracts;

public abstract record PresignedUploadLinkResult
{
    public record Ok(string Url, int UrlValidityInSeconds) : PresignedUploadLinkResult;
    public record Failed(string ErrorMessage) : PresignedUploadLinkResult;

    private PresignedUploadLinkResult() { }
}
