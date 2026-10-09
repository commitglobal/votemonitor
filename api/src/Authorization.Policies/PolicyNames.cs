namespace Authorization.Policies;

public class PolicyNames
{
    public const string PlatformAdminsOnly = nameof(PlatformAdminsOnly);
    public const string NgoAdminsOnly = nameof(NgoAdminsOnly);
    public const string AdminsOnly = nameof(AdminsOnly);
    public const string ObserversOnly = nameof(ObserversOnly);
    public const string NgoAdminOrStaff = nameof(NgoAdminOrStaff);
    public const string NotObservers = nameof(NotObservers);
}
