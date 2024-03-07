using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;

namespace Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;

public class SectionUniquenessRequestValidator : Validator<SectionRequest>
{
    public SectionUniquenessRequestValidator() : this([])
    {
    }

    public SectionUniquenessRequestValidator(IEnumerable<SectionRequest> sections)
    {
        RuleFor(x => x.Code)
            .Must(code => sections.All(s => !string.Equals(s.Code, code, StringComparison.InvariantCultureIgnoreCase)));

        RuleFor(x => x.Questions)
            .Must(questions =>
            {
                var allIds = questions
                    .Union(sections.SelectMany(s => s.Questions))
                    .Select(x => x.Id)
                    .ToList();

                var groupedIds = allIds.GroupBy(x => x, (id, group) => new { Id = id, Count = group.Count() });

                return groupedIds.All(x => x.Count == 1);
            })
            .WithMessage("Question ids should be unique")
            .Must(questions =>
            {
                var allCodes = questions.Union(sections.SelectMany(s => s.Questions))
                    .Select(x => x.Code);

                var groupedCodes =
                    allCodes.GroupBy(x => x, (code, group) => new { Code = code, Count = group.Count() });

                return groupedCodes.All(x => x.Count == 1);
            })
            .WithMessage("Question code should be unique");
    }
}
