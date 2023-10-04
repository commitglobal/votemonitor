using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Feature.PollingStation.Repositories;
internal interface IPollingStationRepository:IRepository<Core.Models.PollingStationEf>
{
    IEnumerable<Core.Models.PollingStationEf> GetByTags(Dictionary<string,string> tags);
}
