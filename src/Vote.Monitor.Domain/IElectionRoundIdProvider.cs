namespace Vote.Monitor.Domain;

public interface IElectionRoundIdProvider
{
    void SetElectionRound(Guid electionRoundId);
    Guid GetElectionRoundId();
}
