namespace Authorization.Policies.Specifications;

internal class NgoStaffView
{
    public required Guid NgoId { get; set; }
    public required NgoStatus NgoStatus { get; set; }
    public required Guid NgoStaffId { get; set; }
    public required UserStatus UserStatus { get; set; }
}
