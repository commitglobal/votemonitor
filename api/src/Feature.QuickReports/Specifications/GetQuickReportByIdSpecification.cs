using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Specifications;
public sealed class GetQuickReportByIdSpecification : SingleResultSpecification<QuickReport>
{
    public GetQuickReportByIdSpecification(Guid electionRoundId, Guid quickReportId)
    {
        Query
            .Where(qr => qr.ElectionRoundId == electionRoundId && qr.Id == quickReportId)
            .Include(x => x.PollingStation)
            .Include(x => x.MonitoringObserver)
            .ThenInclude(x => x.Observer)
            .ThenInclude(x => x.ApplicationUser);
    }

    public GetQuickReportByIdSpecification(Guid electionRoundId, Guid ngoId, Guid quickReportId)
    {
        Query
            .Where(qr =>
                qr.ElectionRoundId == electionRoundId
                && qr.Id == quickReportId
                && qr.MonitoringObserver.MonitoringNgo.NgoId == ngoId);
    }
}
