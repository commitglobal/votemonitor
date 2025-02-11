using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;

namespace Feature.FormTemplates.AssignTemplates;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.FormTemplateIds).NotEmpty();
        RuleForEach(x => x.FormTemplateIds).NotEmpty();
    }
}
