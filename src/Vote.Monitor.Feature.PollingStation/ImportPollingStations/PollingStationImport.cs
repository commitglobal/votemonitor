using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vote.Monitor.Feature.PollingStation.ImportPollingStations;
internal class PollingStationImport
{
    public int DisplayOrder { get; set; }
    public string Address { get; set; }
    public Dictionary<string,string> Tags { get; set; }
}
