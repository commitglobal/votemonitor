namespace Vote.Monitor.Core.Services.Time;

public class TimeFreeze : ITimeService
{
    private readonly DateTime _time;

    public TimeFreeze(DateTime utcTime)
    {
        _time = utcTime.Kind == DateTimeKind.Utc
            ? utcTime
            : throw new ArgumentException("Time must be UTC", nameof(utcTime));
    }

    public TimeFreeze(ITimeService timeService)
    {
        _time = timeService.UtcNow;
    }

    public DateTime UtcNow => _time;
}
