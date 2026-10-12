using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.CitizenReports.Specifications;

public sealed class GetFormReadOnlySpecification : SingleResultSpecification<FormAggregate>
{
    public GetFormReadOnlySpecification(Guid electionRondId, Guid formId)
    {
        Query.AsNoTracking();

        Query.Where(x => x.ElectionRoundId == electionRondId && x.Id == formId)
            .Where(x => x.FormType == FormType.CitizenReporting);
    }
}