using System.Threading.RateLimiting;

namespace Vote.Monitor.Module.Notifications.Contracts;

public interface IPushNotificationRateLimiter
{
    RateLimiter Limiter { get; }
}