namespace Authorization.Policies.Specifications;

internal class NgoAdminView
{
    public required Guid NgoId { get; set; }
    public required NgoStatus NgoStatus { get; set; }
    public required Guid NgoAdminId { get; set; }
    public required UserStatus UserStatus { get; set; }
}
