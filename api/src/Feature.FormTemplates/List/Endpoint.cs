using Authorization.Policies.Requirements;
using Feature.FormTemplates.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Services.Security;

namespace Feature.FormTemplates.List;

public class Endpoint(
    IReadRepository<FormTemplateAggregate> repository,
    ICurrentUserRoleProvider userRoleProvider,
    IAuthorizationService authorizationService)
    : Endpoint<Request, Results<Ok<PagedResponse<FormTemplateSlimModel>>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/form-templates");
        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<FormTemplateSlimModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
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

        var specification = new ListFormTemplatesSpecification(req, isNgoAdmin);
        var formTemplates = await repository.ListAsync(specification, ct);
        var formTemplateCount = await repository.CountAsync(specification, ct);

        return TypedResults.Ok(new PagedResponse<FormTemplateSlimModel>(formTemplates, formTemplateCount,
            req.PageNumber, req.PageSize));
    }
}