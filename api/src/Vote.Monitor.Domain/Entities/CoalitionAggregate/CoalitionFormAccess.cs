using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

namespace Vote.Monitor.Domain.Entities.CoalitionAggregate;

public class CoalitionFormAccess
{
    public Guid CoalitionId { get; set; }
    public Coalition Coalition { get; set; }

    public Guid MonitoringNgoId { get; set; }
    public MonitoringNgo MonitoringNgo { get; set; }

    public Guid FormId { get; set; }
    public Form Form { get; set; }

    public static CoalitionFormAccess Create(Guid coalitionId, Guid monitoringNgoId, Guid formId) => new()
    {
        CoalitionId = coalitionId, MonitoringNgoId = monitoringNgoId, FormId = formId
    };

    public static CoalitionFormAccess Create(Coalition coalition,
        MonitoringNgo monitoringNgo,
        Form form) => new()
    {
        Form = form,
        FormId = form.Id,
        Coalition = coalition,
        CoalitionId = coalition.Id,
        MonitoringNgo = monitoringNgo,
        MonitoringNgoId = monitoringNgo.Id
    };

#pragma warning disable CS8618 // Required by Entity Framework
    private CoalitionFormAccess()
    {
    }
#pragma warning restore CS8618
}
