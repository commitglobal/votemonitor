using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Vote.Monitor.Api.Feature.Form.Specifications;

public sealed class GetFormSpecification : Specification<FormAggregate>
{
    public GetFormSpecification(Guid formId, string code, FormType formType)
    {
        Query
            .Where(x => x.Id != formId && x.Code == code && x.FormType == formType);
    }

    public GetFormSpecification(Guid electionRoundId, Guid monitoringNgoId, string code, FormType formType)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.MonitoringNgoId == monitoringNgoId
                        && x.Code == code
                        && x.FormType == formType);
    }
}
