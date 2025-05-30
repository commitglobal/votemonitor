﻿using Authorization.Policies;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Feature.NgoAdmins.Create;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<NgoAdminAggregate> repository)
    : Endpoint<Request, Results<Ok<NgoAdminModel>, ProblemDetails>>
{
    public override void Configure()
    {
        Post("/api/ngos/{ngoId}/admins");
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
        Summary(x => { x.Description = "Creates ngo admin for a given ngo"; });
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<NgoAdminModel>, ProblemDetails>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email.Trim());
        if (user is not null)
        {
            AddError(r => r.Email, "A user with same login already exists");
            return new ProblemDetails(ValidationFailures);
        }

        var applicationUser =
            ApplicationUser.CreateNgoAdmin(req.FirstName, req.LastName, req.Email, req.PhoneNumber, req.Password);

        var result = await userManager.CreateAsync(applicationUser);
        if (!result.Succeeded)
        {
            AddError(r => r.Email, result.GetAllErrors());
            return new ProblemDetails(ValidationFailures);
        }

        var ngoAdmin = new NgoAdminAggregate(req.NgoId, applicationUser);
        await repository.AddAsync(ngoAdmin, ct);

        return TypedResults.Ok(new NgoAdminModel
        {
            Id = ngoAdmin.Id,
            FirstName = applicationUser.FirstName,
            LastName = applicationUser.LastName,
            Email = applicationUser.Email!,
            PhoneNumber = applicationUser.PhoneNumber,
            Status = applicationUser.Status,
            CreatedOn = ngoAdmin.CreatedOn,
            LastModifiedOn = ngoAdmin.LastModifiedOn
        });
    }
}
