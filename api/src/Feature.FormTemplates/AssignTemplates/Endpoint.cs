using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Domain;
using Vote.Monitor.Domain.Entities.ElectionRoundFormTemplateAggregate;

namespace Feature.FormTemplates.AssignTemplates;

public class Endpoint(
    VoteMonitorContext context,
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
        var electionRound = await context.ElectionRounds.FindAsync(req.ElectionRoundId);

        if (electionRound is null)
        {
            return TypedResults.NotFound();
        }
        
        // obtinem toate asignarile de template-uri pentru electionRound
        
        var existingAssignments = await context.ElectionRoundFormTemplates
            .Where(x => x.ElectionRoundId == req.ElectionRoundId)
            .ToListAsync();
        
        var existingTemplateIds = existingAssignments
            .Select(x => x.FormTemplateId)
            .ToList();
        var requestedTemplateIds = req.FormTemplateIds.ToList();
        
        //  definim cazurile
        var templatesToAdd = requestedTemplateIds.Except(existingTemplateIds).ToList();
        var templatesToRemove = existingTemplateIds.Except(requestedTemplateIds).ToList();
        
        // stergem asignarile care nu se regasesc in request
        if (templatesToRemove.Any())
        {
            var assignmentsToDelete = existingAssignments
                .Where(x => templatesToRemove.Contains(x.FormTemplateId))
                .ToList();
            await electionRoundFormTemplateRepository.DeleteRangeAsync(assignmentsToDelete);
        }
        
        // adaugam noile asignari
        if (templatesToAdd.Any())
        {
            var newAssignments = templatesToAdd
                .Select(templateId => ElectionRoundFormTemplate.Create(req.ElectionRoundId, templateId))
                .ToList();
            await electionRoundFormTemplateRepository.AddRangeAsync(newAssignments);
        }
        
        // salvam modificarile in baza de date
        await context.SaveChangesAsync();

        return TypedResults.NoContent();
        
        /*
         * Suntem intr-un election round fara template-uri assignate
         * - assignam template-uri pt un election round
         * case 2:
         *  - avem assignate pt o runda de alegeri template-urile A,B,C si vine requestul A,B,C,D ?:
         *          -> (idempotenta:de citit) atunci o sa avem rezultatul A,B,C,D
         * case 3:
         *  - avem assignate pt o runda de alegeri template-urile A,B,C si vine requestul D ?:
         *           -> formtemplateAssignmentRezultat va fi doar D
           case 4:
            - avem assignate pt o runda de alegeri template-urile A,B,C si vine requestul empty ?:
                    -> se vor sterge toate (A,B,C) formTemplateAssignment-urile 
           case 5:
            - avem assignate pt o runda de alegeri template-urile A,B,C si vine requestul B,C,D ?:
                    -> vom avea rezultatul B,C,D
         */
        // Feature.Coalitions.FormAccess ( implementare ~~~~)
    }
}
