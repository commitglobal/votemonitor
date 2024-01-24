namespace Vote.Monitor.Core.Services.ElectionRound;

public interface IElectionRoundIdProvider
{
    void SetElectionRound(Guid electionRoundId);
    Guid GetElectionRoundId();
}
