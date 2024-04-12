namespace Vote.Monitor.Core.Services.Time;

public class CurrentUtcTimeProvider : ITimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateOnly UtcNowDate => DateOnly.FromDateTime(UtcNow);

    public static CurrentUtcTimeProvider Instance => new ();
}
