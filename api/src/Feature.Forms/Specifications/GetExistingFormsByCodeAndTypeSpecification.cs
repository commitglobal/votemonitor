using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.Specifications;

public sealed class GetExistingFormsByCodeAndTypeSpecification : Specification<FormAggregate>
{
    public GetExistingFormsByCodeAndTypeSpecification(Guid electionRoundId, Guid monitoringNgoId, Guid formId, string code, FormType formType)
    {
        Query
            .Where(x => x.Id != formId
                        && x.ElectionRoundId == electionRoundId
                        && x.MonitoringNgoId == monitoringNgoId
                        && x.Code == code
                        && x.FormType == formType);
    }

    public GetExistingFormsByCodeAndTypeSpecification(Guid electionRoundId, Guid monitoringNgoId, string code, FormType formType)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId
                        && x.MonitoringNgoId == monitoringNgoId
                        && x.Code == code
                        && x.FormType == formType);
    }
}
