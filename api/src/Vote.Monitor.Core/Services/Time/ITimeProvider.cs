namespace Vote.Monitor.Core.Services.Time;

public interface ITimeProvider
{
    DateTime UtcNow { get; }
    DateOnly UtcNowDate { get; }
}
