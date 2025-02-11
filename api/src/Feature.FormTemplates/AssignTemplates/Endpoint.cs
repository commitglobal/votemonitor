using Feature.FormTemplates.Specifications;
using Vote.Monitor.Domain.Entities.FormTemplateAggregate;

namespace Feature.FormTemplates.AssignTemplates;

public class Endpoint(IRepository<FormTemplate> repository) :
        Endpoint<Request, Results<NoContent, NotFound>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/form-templates:assign-templates");
        Policies(PolicyNames.PlatformAdminsOnly);
    }

    public override async Task<Results<NoContent, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
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

        return null;
    }
}
