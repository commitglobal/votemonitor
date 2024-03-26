using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

public class ObserverGuide : AuditableBaseEntity, IAggregateRoot
{
    public Guid MonitoringNgoId { get; private set; }
    public MonitoringNgo MonitoringNgo { get; private set; }
    public string Title { get; private set; }
    public string FileName { get; private set; }
    public string UploadedFileName { get; private set; }
    public string FilePath { get; private set; }
    public string MimeType { get; private set; }
    public bool IsDeleted { get; private set; }

    public ObserverGuide(MonitoringNgo monitoringNgo,
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
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal ObserverGuide()
    {
    }
#pragma warning restore CS8618

    public void Delete()
    {
        IsDeleted = true;
    }
}
