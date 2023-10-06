using Vote.Monitor.Core;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal interface IPollingStationRepository:IRepository<PollingStationModel>
{
    Task<IEnumerable<PollingStationModel>> GetByTags(Dictionary<string,string> tags);

    Task<IEnumerable<TagModel>> GetTags();
}
