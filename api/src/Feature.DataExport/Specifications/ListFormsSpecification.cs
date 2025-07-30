using Ardalis.Specification;
using Feature.DataExport.Export;
using Module.Forms.Mappers;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.DataExport.Specifications;

public sealed class ListFormsSpecification : Specification<Form>
{
    public ListFormsSpecification(Guid electionRoundId, Guid ngoId)
    {
        Query.Where(x => x.ElectionRoundId == electionRoundId
                         && x.MonitoringNgo.NgoId == ngoId 
                         && x.FormType != FormType.CitizenReporting 
                         && x.Status == FormStatus.Published);
    }
}
