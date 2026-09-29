using Authorization.Policies.Requirements;
using Feature.FormTemplates.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Feature.FormTemplates.ListAssignedTemplates;

public class Endpoint(IReadRepository<ElectionRoundFormTemplate> repository,
    ICurrentUserRoleProvider userRoleProvider,
    IAuthorizationService authorizationService)
    : Endpoint<Request, Results<Ok<List<FormTemplateSlimModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/form-templates/election-rounds/{electionRoundId}:available");
        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task<Results<Ok<List<FormTemplateSlimModel>>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var isNgoAdmin = userRoleProvider.IsNgoAdmin();

        if (isNgoAdmin)
        {
            var result = await authorizationService.AuthorizeAsync(User, new NgoAdminRequirement());

            if (!result.Succeeded)
            {
                return TypedResults.NotFound();
            }
        }

        var specification = new ListAssignedFormTemplateSpecification(req);
        var assignedFormTemplates = await repository.ListAsync(specification, ct);

        return TypedResults.Ok(assignedFormTemplates);
    }
}
