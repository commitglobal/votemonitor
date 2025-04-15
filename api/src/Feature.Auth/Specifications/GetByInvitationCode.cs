using Ardalis.Specification;

namespace Feature.Auth.Specifications;

public sealed class GetByInvitationCode: SingleResultSpecification<ApplicationUser>
{
    public GetByInvitationCode(string invitationToken)
    {
        Query.Where(x => x.InvitationToken == invitationToken);
    }
}
