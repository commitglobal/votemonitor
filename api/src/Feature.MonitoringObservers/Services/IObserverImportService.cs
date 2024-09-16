using Feature.MonitoringObservers.Parser;

namespace Feature.MonitoringObservers.Services;

public interface IObserverImportService
{
    Task ImportAsync(Guid electionRoundId, Guid ngoId, IEnumerable<MonitoringObserverImportModel> newObservers,
        CancellationToken ct);
}