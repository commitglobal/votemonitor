using Ardalis.Specification;

namespace Vote.Monitor.Api.Feature.Auth.Specifications;

public sealed class GetByInvitationCode: SingleResultSpecification<ApplicationUser>
{
    public GetByInvitationCode(string invitationToken)
    {
        Query.Where(x => x.InvitationToken == invitationToken);
    }
}
