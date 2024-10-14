using Authorization.Policies.Requirements;
using Feature.FormTemplates.Specifications;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Core.Services.Security;

namespace Feature.FormTemplates.Get;

public class Endpoint(
    IReadRepository<FormTemplateAggregate> repository,
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

        var formTemplate = await repository.SingleOrDefaultAsync(new GetFormTemplateSpecification(req.Id, isNgoAdmin), ct);

        if (formTemplate is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new FormTemplateFullModel
        {
            Id = formTemplate.Id,
            FormTemplateType = formTemplate.FormTemplateType,
            Code = formTemplate.Code,
            DefaultLanguage = formTemplate.DefaultLanguage,
            Name = formTemplate.Name,
            Description = formTemplate.Description,
            Status = formTemplate.Status,
            CreatedOn = formTemplate.CreatedOn,
            LastModifiedOn = formTemplate.LastModifiedOn,
            Languages = formTemplate.Languages,
            NumberOfQuestions = formTemplate.NumberOfQuestions,
            Questions = formTemplate.Questions.Select(QuestionsMapper.ToModel).ToList()
        });
    }
}