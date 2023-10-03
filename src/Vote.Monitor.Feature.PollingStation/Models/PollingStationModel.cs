using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vote.Monitor.Feature.PollingStation.Models;
internal class PollingStationModel
{
    public int Id { get; set; }
    public required string Address { get; set; }
    public int DisplayOrder { get; set; }
    public Dictionary<string, string> Tags { get; set; }

    public PollingStationModel()
    {
        Tags = new Dictionary<string, string>();
    }
}
