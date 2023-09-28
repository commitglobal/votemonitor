using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vote.Monitor.Feature.PollingStation.CreatePollingStation;
internal class PollingStationCreateRequestDto
{
    public required string Address { get; set; }
    public int DisplayOrder { get; set; }
    public Dictionary<string, string> Tags { get; set; }

    public PollingStationCreateRequestDto()
    {
        Tags = new Dictionary<string, string>();
    }   
   
}
