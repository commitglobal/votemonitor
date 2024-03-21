namespace Vote.Monitor.Domain.Entities.MonitoringNgoAggregate;

public interface IMonitoringNgoRepository
{
    Task<(Guid id, MonitoringNgoStatus status)?> GetMonitoringNgoStatusAsync(Guid electionId, Guid ngoId);
}

public class MonitoringNgoRepository : IMonitoringNgoRepository
{
    private readonly VoteMonitorContext _context;

    public MonitoringNgoRepository(VoteMonitorContext context)
    {
        _context = context;
    }

    public async Task<(Guid id, MonitoringNgoStatus status)?> GetMonitoringNgoStatusAsync(Guid electionId, Guid ngoId)
    {
        var monitoringNgo = await _context.MonitoringNgos
            .Where(x => x.ElectionRoundId == electionId && x.NgoId == ngoId)
            .Select(ngo => new { ngo.Id, ngo.Status })
            .FirstOrDefaultAsync();


        return monitoringNgo != null ? (monitoringNgo.Id, monitoringNgo.Status) : null;
    }
}
