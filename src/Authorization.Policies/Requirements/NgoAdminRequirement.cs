namespace Authorization.Policies.Requirements;

public class NgoAdminRequirement(Guid ngoId) : IAuthorizationRequirement
{
    public Guid NgoId { get; } = ngoId;
}
