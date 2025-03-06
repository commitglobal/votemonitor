using Feature.Monitoring.List;

namespace Feature.Monitoring.Specifications;

public sealed class ListMonitoringNgosSpecification : SingleResultSpecification<MonitoringNgoAggregate, MonitoringNgoModel>
{
    public ListMonitoringNgosSpecification(Guid electionRoundId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId);

        Query.Select(x => new MonitoringNgoModel
        {
            Id = x.Id,
            NgoId = x.NgoId,
            Name = x.Ngo.Name,
            NgoStatus = x.Ngo.Status,
            MonitoringNgoStatus = x.Status
        });
    }
}
