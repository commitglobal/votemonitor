using Vote.Monitor.Feature.PollingStation.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal interface IPollingStationRepository : IRepository<PollingStationModel>
{
    Task<IEnumerable<PollingStationModel>> GetByTags(Dictionary<string, string> tags);
}
