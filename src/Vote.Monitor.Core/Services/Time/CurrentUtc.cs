namespace Vote.Monitor.Core.Services.Time;

public class CurrentUtc : ITimeService
{
    public DateTime UtcNow => DateTime.UtcNow;

    public static CurrentUtc Instance => new ();
}
