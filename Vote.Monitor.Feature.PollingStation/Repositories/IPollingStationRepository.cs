using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vote.Monitor.Feature.PollingStation.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal interface IPollingStationRepository:IRepository<PollingStationModel>
{
    IEnumerable<PollingStationModel> GetByTags(Dictionary<string,string> tags);
}
