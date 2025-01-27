using Vote.Monitor.Domain.Entities.NgoAggregate;

namespace Vote.Monitor.Api.Feature.Ngo;

public record NgoModel
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public NgoStatus Status { get; init; }
    public int NumberOfNgoAdmins { get; init; }
    public int NumberOfElectionsMonitoring { get; init; }
    public DateOnly? DateOfLastElection { get; set; }
}
