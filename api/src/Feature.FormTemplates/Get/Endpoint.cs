﻿using Authorization.Policies.Requirements;
using Feature.FormTemplates.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.Get;

public class Endpoint(
    IReadRepository<FormTemplate> repository,
    ICurrentUserRoleProvider userRoleProvider,
    IAuthorizationService authorizationService) : Endpoint<Request, Results<Ok<FormTemplateFullModel>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/form-templates/{id}");
        Policies(PolicyNames.AdminsOnly);
    }

    public override async Task<Results<Ok<FormTemplateFullModel>, NotFound>> ExecuteAsync(Request req,
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

        var formTemplate =
            await repository.SingleOrDefaultAsync(new GetFormTemplateSpecification(req.Id, isNgoAdmin), ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(FormTemplateFullModel.FromEntity(formTemplate));
    }
}
