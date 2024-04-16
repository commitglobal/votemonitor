
namespace Vote.Monitor.Core.Services.Security;

public interface ICurrentUserRoleProvider 
{ 
    Task<Guid?> GetNgoId();
    bool IsPlatformAdmin();
    bool IsNgoAdmin();
    bool IsObserver();
}
