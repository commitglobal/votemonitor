using Ardalis.Specification;
using Vote.Monitor.Domain.Entities.QuickReportAggregate;

namespace Feature.QuickReports.Specifications;
public sealed class GetQuickReportByIdSpecification : SingleResultSpecification<QuickReport>
{
    public GetQuickReportByIdSpecification(Guid electionRoundId, Guid quickReportId)
    {
        Query.Where(qr => qr.ElectionRoundId == electionRoundId && qr.Id == quickReportId);
    }
}
