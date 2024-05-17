
namespace Vote.Monitor.Core.Services.Security;

public interface ICurrentUserRoleProvider 
{
    bool IsPlatformAdmin();
    bool IsNgoAdmin();
    bool IsObserver();
}
