using Vote.Monitor.Core.Services.Security;

namespace Feature.IncidentReports.GetById;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.IncidentReportId).NotEmpty();

        // Platform admins have no NGO; an NGO admin's or NGO staff member's NGO comes from their claims and must be there.
        // NotEmpty() on a Guid? only rejects null, so Guid.Empty is checked too.
        RuleFor(x => x.NgoId)
            .NotNull()
            .NotEqual(Guid.Empty)
            .When(_ =>
            {
                var roles = Resolve<ICurrentUserRoleProvider>();
                return roles.IsNgoAdmin() || roles.IsNgoStaff();
            });
    }
}
