using System.Threading.RateLimiting;
using Vote.Monitor.Module.Notifications.Contracts;

namespace Vote.Monitor.Module.Notifications.Expo;

public class ExpoPushNotificationRateLimiter(RateLimiter limiter) : IPushNotificationRateLimiter
{
    public RateLimiter Limiter { get; } = limiter;
}