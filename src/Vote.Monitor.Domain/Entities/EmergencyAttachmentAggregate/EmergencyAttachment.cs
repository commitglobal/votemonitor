using Vote.Monitor.Domain.Entities.EmergencyAggregate;

namespace Vote.Monitor.Domain.Entities.EmergencyAttachmentAggregate;

public class EmergencyAttachment : AuditableBaseEntity, IAggregateRoot
{
    public Guid EmergencyId { get; private set; }
    public Emergency Emergency { get; private set; }
    public string Filename { get; private set; }
    public string MimeType { get; private set; }

    public EmergencyAttachment(Emergency emergency,
        string filename,
        string mimeType) : base(Guid.NewGuid())
    {
        Emergency = emergency;
        EmergencyId = emergency.Id;
        Filename = filename;
        MimeType = mimeType;
    }

#pragma warning disable CS8618 // Required by Entity Framework

    internal EmergencyAttachment()
    {
    }
#pragma warning restore CS8618
}
