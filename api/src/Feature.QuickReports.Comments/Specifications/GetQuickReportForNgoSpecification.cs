using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Comments.Specifications;

public sealed class GetQuickReportForNgoSpecification : Specification<QuickReport>
{
    public GetQuickReportForNgoSpecification(Guid electionRoundId, Guid ngoId, Guid quickReportId)
    {
        Query.Where(x => x.Id == quickReportId
                         && x.ElectionRoundId == electionRoundId
                         && x.MonitoringObserver.MonitoringNgo.NgoId == ngoId
                         && x.MonitoringObserver.MonitoringNgo.ElectionRoundId == electionRoundId);
    }
}
