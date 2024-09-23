namespace Vote.Monitor.Domain.Entities.CitizenReportGuideAggregate;

public class CitizenReportGuide : AuditableBaseEntity, IAggregateRoot
{
    public Guid ElectionRoundId { get; private set; }
    public virtual ElectionRound ElectionRound { get; private set; }
    public string Title { get; private set; }
    public string? FileName { get; private set; }
    public string? UploadedFileName { get; private set; }
    public string? FilePath { get; private set; }
    public string? MimeType { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? Text { get; private set; }
    public CitizenReportGuideType GuideType { get; private set; }
    public bool IsDeleted { get; private set; }

    private CitizenReportGuide(Guid electionRoundId,
        string title,
        string fileName,
        string filePath,
        string mimeType) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        Title = title;
        FileName = fileName;
        FilePath = filePath;
        MimeType = mimeType;
        IsDeleted = false;

        var extension = FileName.Split('.').Last();
        var uploadedFileName = $"{Id}.{extension}";
        UploadedFileName = uploadedFileName;
        GuideType = CitizenReportGuideType.Document;
    }

    private CitizenReportGuide(Guid electionRoundId,
        string title,
        Uri websiteUrl) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        Title = title;
        WebsiteUrl = websiteUrl.ToString();
        IsDeleted = false;
        GuideType = CitizenReportGuideType.Website;
    }
    
    private CitizenReportGuide(Guid electionRoundId,
        string title,
        string text) : base(Guid.NewGuid())
    {
        ElectionRoundId = electionRoundId;
        Title = title;
        Text = text;
        IsDeleted = false;
        GuideType = CitizenReportGuideType.Text;
    }

    public static CitizenReportGuide NewDocumentGuide(Guid electionRoundId,
        string title,
        string fileName,
        string filePath,
        string mimeType) => new(electionRoundId, title, fileName, filePath, mimeType);

    public static CitizenReportGuide NewWebsiteGuide(Guid electionRoundId,
        string title,
        Uri websiteUrl) => new(electionRoundId, title, websiteUrl);    
    
    public static CitizenReportGuide NewTextGuide(Guid electionRoundId,
        string title,
        string text) => new(electionRoundId, title, text);

    public void Delete()
    {
        IsDeleted = true;
    }

    public void UpdateTitle(string title)
    {
        Title = title;
    }
    
    public void UpdateWebsiteGuide(string title, Uri websiteUrl)
    {
        Title = title;
        WebsiteUrl = websiteUrl.ToString();
    }
    
    public void UpdateTextGuide(string title, string text)
    {
        Title = title;
        Text = text;
    }

#pragma warning disable CS8618 // Required by Entity Framework
    internal CitizenReportGuide()
    {
    }
#pragma warning restore CS8618
}