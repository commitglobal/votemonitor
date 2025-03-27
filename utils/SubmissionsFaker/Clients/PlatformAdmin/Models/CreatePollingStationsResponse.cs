using SubmissionsFaker.Clients.Models;

namespace SubmissionsFaker.Clients.PollingStations;

public class CreatePollingStationsResponse
{
    public ResponseWithId[] PollingStations { get; set; } = [];
}
