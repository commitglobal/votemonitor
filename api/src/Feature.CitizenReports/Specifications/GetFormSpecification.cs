using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.CitizenReports.Specifications;

public sealed class GetFormSpecification : SingleResultSpecification<FormAggregate>
{
    public GetFormSpecification(Guid electionRondId, Guid formId)
    {
        Query.Where(x => x.ElectionRoundId == electionRondId && x.Id == formId)
            .Where(x => x.FormType == FormType.CitizenReporting);
    }
}