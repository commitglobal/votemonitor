using Vote.Monitor.Core.Security;

namespace Feature.Citizen.Notifications.ListReceived;

public class Request
{
    public Guid ElectionRoundId { get; set; }
}
