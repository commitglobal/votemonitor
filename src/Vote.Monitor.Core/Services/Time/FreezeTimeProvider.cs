namespace Vote.Monitor.Core.Services.Time;

public class FreezeTimeProvider : ITimeProvider
{
    private readonly DateTime _time;

    public FreezeTimeProvider(DateTime utcTime)
    {
        _time = utcTime.Kind == DateTimeKind.Utc
            ? utcTime
            : throw new ArgumentException("Time must be UTC", nameof(utcTime));
    }

    public FreezeTimeProvider(ITimeProvider timeProvider)
    {
        _time = timeProvider.UtcNow;
    }

    public DateTime UtcNow => _time;
    public DateOnly UtcNowDate => DateOnly.FromDateTime(UtcNow);
}
