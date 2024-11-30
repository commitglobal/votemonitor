namespace Feature.ObserverGuide.Model;

public record ObserverGuideModel
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public string FileName { get; init; }
    public string MimeType { get; init; }
    public string GuideType { get; init; }
    public string Text { get; init; }
    public string WebsiteUrl { get; init; }
    public DateTime CreatedOn { get; init; }
    public string CreatedBy { get; init; }
    public bool IsGuideOwner { get; init; }
    public string PresignedUrl { get; init; }
    public int UrlValidityInSeconds { get; init; }
    public string FilePath { get; init; }
    public string UploadedFileName { get; init; }
    public GuideAccessModel[] GuideAccess { get; init; } = [];
}
