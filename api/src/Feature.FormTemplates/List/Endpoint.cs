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
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<PagedResponse<FormTemplateSlimModel>>, NotFound>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var specification = new ListFormTemplatesSpecification(req);
        
        var formTemplates = await repository.ListAsync(specification, ct);
        var formTemplateCount = await repository.CountAsync(specification, ct);

        return TypedResults.Ok(new PagedResponse<FormTemplateSlimModel>(formTemplates, formTemplateCount,
            req.PageNumber, req.PageSize));
    }
}

// Endpoint:
