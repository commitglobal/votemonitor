using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vote.Monitor.Core.Models;
public class PollingStationEf
{
    public int Id { get; set; }
    public required string Address { get; set; }
    public int DisplayOrder { get; set; }
    public List<TagEf> Tags { get; set; } = new List<TagEf>();

    
}
