using Vote.Monitor.Core.Models;

namespace Vote.Monitor.Domain.Entities.PollingStationCorrectionAggregate;

public static class PollingStationCorrectionQueries
{
    /// <summary>The record's corrections, oldest first. Who made them is never returned.</summary>
    public static Task<PollingStationCorrectionModel[]> GetPollingStationCorrectionsAsync(this VoteMonitorContext context,
        Guid electionRoundId,
        CorrectableRecordType recordType,
        Guid recordId,
        CancellationToken ct) =>
        context.PollingStationCorrections
            .AsNoTracking()
            .Where(x => x.ElectionRoundId == electionRoundId && x.RecordType == recordType && x.RecordId == recordId)
            .OrderBy(x => x.CorrectedAt)
            .Select(x => new PollingStationCorrectionModel
            {
                CorrectedAt = x.CorrectedAt,
                From = new StationRefModel
                {
                    Id = x.FromPollingStation.Id,
                    Number = x.FromPollingStation.Number,
                    Level1 = x.FromPollingStation.Level1,
                    Level2 = x.FromPollingStation.Level2,
                    Level3 = x.FromPollingStation.Level3,
                    Level4 = x.FromPollingStation.Level4,
                    Level5 = x.FromPollingStation.Level5,
                    Address = x.FromPollingStation.Address
                },
                To = new StationRefModel
                {
                    Id = x.ToPollingStation.Id,
                    Number = x.ToPollingStation.Number,
                    Level1 = x.ToPollingStation.Level1,
                    Level2 = x.ToPollingStation.Level2,
                    Level3 = x.ToPollingStation.Level3,
                    Level4 = x.ToPollingStation.Level4,
                    Level5 = x.ToPollingStation.Level5,
                    Address = x.ToPollingStation.Address
                }
            })
            .ToArrayAsync(ct);
}
