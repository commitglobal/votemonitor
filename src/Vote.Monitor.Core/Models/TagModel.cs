using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vote.Monitor.Core.Models;
public  class TagModel
{
    [Key] public int Id { get; set; }

    public required string Key { get; set; }
    public required string Value { get; set; }

    public List<PollingStationModel> PollingStations { get; } = new();
}
