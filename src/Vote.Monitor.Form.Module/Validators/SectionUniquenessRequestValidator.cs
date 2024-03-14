using FastEndpoints;
using FluentValidation;
using Vote.Monitor.Form.Module.Requests;

namespace Vote.Monitor.Form.Module.Validators;

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
                var allIds = Enumerable
                    .ToList<Guid>(questions
                        .Union(sections.SelectMany(s => s.Questions))
                        .Select(x => x.Id));

                var groupedIds = allIds.GroupBy(x => x, (id, group) => new { Id = id, Count = group.Count() });

                return groupedIds.All(x => x.Count == 1);
            })
            .WithMessage("Question ids should be unique")
            .Must(questions =>
            {
                var allCodes = questions.Union(sections.SelectMany(s => s.Questions))
                    .Select(x => x.Code);

                var groupedCodes =
                    Enumerable.GroupBy(allCodes, x => x, (code, group) => new { Code = code, Count = Enumerable.Count<string>(group) });

                return groupedCodes.All(x => x.Count == 1);
            })
            .WithMessage("Question code should be unique");
    }
}
