using Ardalis.Specification;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Feature.DataExport.Specifications;

public sealed class ListPollingStationsSpecification : Specification<PollingStation, PollingStationModel>
{
    public ListPollingStationsSpecification(Guid electionRoundId)
    {
        Query
            .Where(x => x.ElectionRoundId == electionRoundId);
        
        Query
            .Select(x => new PollingStationModel
            {
                Id = x.Id,
                Level1 = x.Level1,
                Level2 = x.Level2,
                Level3 = x.Level3,
                Level4 = x.Level4,
                Level5 = x.Level5,
                Number = x.Number,
                Address = x.Address,
                DisplayOrder = x.DisplayOrder
            });
    }
}
