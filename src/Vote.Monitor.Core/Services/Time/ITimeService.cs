namespace Vote.Monitor.Core.Services.Time;

public interface ITimeService
{
    DateTime UtcNow { get; }
}
