using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Ngo;

public record NgoModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public NgoStatus Status { get; init; }
    public int NumberOfNgoAdmins { get; init; }
    public int NumberOfMonitoredElections { get; init; }

    public MonitoredElectionsModel[] MonitoredElections { get; init; } = [];
    public DateOnly? DateOfLastElection { get; set; }
    
    public class MonitoredElectionsModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string EnglishTitle { get; set; }
        public DateOnly StartDate { get; set; }
        public ElectionRoundStatus Status { get; set; }
    }
}
