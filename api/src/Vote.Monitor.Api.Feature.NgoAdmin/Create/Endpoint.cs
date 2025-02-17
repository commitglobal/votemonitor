using Authorization.Policies;
using Microsoft.AspNetCore.Identity;
using Vote.Monitor.Core.Extensions;

namespace Vote.Monitor.Api.Feature.NgoAdmin.Create;

public class Endpoint(
    UserManager<ApplicationUser> userManager,
    IRepository<NgoAdminAggregate> repository)
    : Endpoint<Request, Results<Ok<NgoAdminModel>, Conflict<ProblemDetails>>>
{
    public override void Configure()
    {
        Post("/api/ngos/{ngoId}/admins");
        DontAutoTag();
        Options(x => x.WithTags("ngo-admins"));
        Summary(x => { x.Description = "Creates ngo admin for a given ngo"; });
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<Ok<NgoAdminModel>, Conflict<ProblemDetails>>> ExecuteAsync(Request req,
        CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        if (user is not null)
        {
            AddError(r => r.Email, "A ngo admin with same login already exists");
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
        }

        var applicationUser =
            ApplicationUser.CreateNgoAdmin(req.FirstName, req.LastName, req.Email, req.PhoneNumber, req.Password);

        var result = await userManager.CreateAsync(applicationUser);
        if (!result.Succeeded)
        {
            AddError(r => r.Email, result.GetAllErrors());
            return TypedResults.Conflict(new ProblemDetails(ValidationFailures));
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
