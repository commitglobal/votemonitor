
namespace Vote.Monitor.Core.Services.Security;

public interface ICurrentUserProvider
{
    Guid? GetUserId();
    Guid? GetNgoId();
    bool IsPlatformAdmin();
    bool IsNgoAdmin();
    bool IsObserver();

}
