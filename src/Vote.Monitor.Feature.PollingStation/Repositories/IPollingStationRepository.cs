using Vote.Monitor.Core;
using Vote.Monitor.Domain.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal interface IPollingStationRepository:IRepositoryCommand<PollingStationModel>, IRepositoryQuery<PollingStationModel, TagModel>
{

}
