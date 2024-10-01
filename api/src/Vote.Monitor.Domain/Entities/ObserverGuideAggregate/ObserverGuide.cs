using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

public class ObserverGuide : AuditableBaseEntity, IAggregateRoot
{
    public Guid MonitoringNgoId { get; private set; }
    public MonitoringNgo MonitoringNgo { get; private set; }
    public string Title { get; private set; }
    public string? FileName { get; private set; }
    public string? UploadedFileName { get; private set; }
    public string? FilePath { get; private set; }
    public string? MimeType { get; private set; }
    public string? WebsiteUrl { get; private set; }
    public string? Text { get; private set; }
    public ObserverGuideType GuideType { get; private set; }
    public bool IsDeleted { get; private set; }

    private ObserverGuide(MonitoringNgo monitoringNgo,
        string title,
        string fileName,
        string filePath,
        string mimeType) : base(Guid.NewGuid())
    {
        MonitoringNgo = monitoringNgo;
        MonitoringNgoId = monitoringNgo.Id;
        Title = title;
        FileName = fileName;
        FilePath = filePath;
        MimeType = mimeType;
        IsDeleted = false;

        var extension = FileName.Split('.').Last();
        var uploadedFileName = $"{Id}.{extension}";
        UploadedFileName = uploadedFileName;
        GuideType = ObserverGuideType.Document;
    }

    private ObserverGuide(MonitoringNgo monitoringNgo,
        string title,
        Uri websiteUrl) : base(Guid.NewGuid())
    {
        MonitoringNgo = monitoringNgo;
        MonitoringNgoId = monitoringNgo.Id;
        Title = title;
        WebsiteUrl = websiteUrl.ToString();
        IsDeleted = false;
        GuideType = ObserverGuideType.Website;
    }

    private ObserverGuide(MonitoringNgo monitoringNgo,
        string title,
        string text) : base(Guid.NewGuid())
    {
        MonitoringNgo = monitoringNgo;
        MonitoringNgoId = monitoringNgo.Id;
        Title = title;
        Text = text;
        IsDeleted = false;
        GuideType = ObserverGuideType.Text;
    }

    public static ObserverGuide NewDocumentGuide(MonitoringNgo monitoringNgo,
        string title,
        string fileName,
        string filePath,
        string mimeType) => new(monitoringNgo, title, fileName, filePath, mimeType);

    public static ObserverGuide NewWebsiteGuide(MonitoringNgo monitoringNgo,
        string title,
        Uri websiteUrl) => new(monitoringNgo, title, websiteUrl);

    public static ObserverGuide NewTextGuide(MonitoringNgo monitoringNgo,
        string title,
        string text) => new(monitoringNgo, title, text);

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
    internal ObserverGuide()
    {
    }
#pragma warning restore CS8618
}