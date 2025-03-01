using Feature.FormTemplates.Specifications;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Feature.FormTemplates.AssignTemplates;

public class Endpoint(
    IReadRepository<ElectionRound> electionRoundRepository,
    IReadRepository<FormTemplateAggregate> formTemplateRepository,
    IRepository<ElectionRoundFormTemplate> electionRoundFormTemplateRepository
) :
    Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-templates:assign-templates");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRoundExists =
            await electionRoundRepository.AnyAsync(new GetElectionRoundByIdSpecification(req.ElectionRoundId), ct);

        if (electionRoundExists is false)
        {
            return TypedResults.NotFound();
        }

        if (await formTemplateRepository.CountAsync(new ListFormTemplatesByIds(req.FormTemplateIds)) !=
            req.FormTemplateIds.Count)
        {
            return TypedResults.NotFound();
        }

        var existingAssignments = await electionRoundFormTemplateRepository
            .ListAsync(new ListElectionRoundFormTemplateSpecification(req.ElectionRoundId), ct);

        var existingTemplateIds = existingAssignments
            .Select(x => x.FormTemplateId)
            .ToList();

        var templatesToAdd = req.FormTemplateIds.Except(existingTemplateIds).ToList();
        var templatesToRemove = existingTemplateIds.Except(req.FormTemplateIds).ToList();

        if (templatesToRemove.Any())
        {
            var assignmentsToDelete = existingAssignments
                .Where(x => templatesToRemove.Contains(x.FormTemplateId))
                .ToList();
            await electionRoundFormTemplateRepository.DeleteRangeAsync(assignmentsToDelete, ct);
        }

        if (templatesToAdd.Any())
        {
            var newAssignments = templatesToAdd
                .Select(templateId => ElectionRoundFormTemplate.Create(req.ElectionRoundId, templateId))
                .ToList();
            await electionRoundFormTemplateRepository.AddRangeAsync(newAssignments, ct);
        }

        return TypedResults.NoContent();
    }
}
