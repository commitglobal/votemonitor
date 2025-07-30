using Ardalis.Specification;
using Feature.DataExport.Export;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;

namespace Feature.DataExport.Specifications;

public sealed class ListGuidesSpecification : Specification<ObserverGuide, Response.GuideModel>
{
    public ListGuidesSpecification(Guid electionRoundId, Guid ngoId)
    {
        Query.Where(x =>
            x.MonitoringNgo.ElectionRoundId == electionRoundId
            && x.MonitoringNgo.NgoId == ngoId
            && x.IsDeleted == false
            && x.GuideType == ObserverGuideType.Text);
        Query.Select(g => new Response.GuideModel { Id = g.Id, Title = g.Title, Text = g.Text, });
    }
}
