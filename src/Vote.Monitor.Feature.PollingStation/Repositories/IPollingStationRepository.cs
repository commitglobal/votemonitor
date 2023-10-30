using Vote.Monitor.Core;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
public interface IPollingStationRepository : IRepositoryCommand<Domain.Models.PollingStation>, IRepositoryQuery<Domain.Models.PollingStation>
{
}
