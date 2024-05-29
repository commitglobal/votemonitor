namespace Feature.Statistics.Get;

public class ObserversStats
{
    public int ActiveObservers { get; set; }
    public int PendingObservers { get; set; }
    public int SuspendedObservers { get; set; }

    public int TotalNumberOfObservers => ActiveObservers + PendingObservers + SuspendedObservers;
}
