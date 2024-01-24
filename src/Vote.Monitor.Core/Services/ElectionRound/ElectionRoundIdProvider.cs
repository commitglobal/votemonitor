namespace Vote.Monitor.Core.Services.ElectionRound;

public class ElectionRoundIdProvider : IElectionRoundIdProvider
{
    private Guid? _electionRoundId;

    public void SetElectionRound(Guid electionRoundId)
    {
        _electionRoundId = electionRoundId;
    }

    public Guid GetElectionRoundId()
    {
        return _electionRoundId ?? throw new ArgumentNullException(nameof(_electionRoundId), "Election round id not set");
    }
}
