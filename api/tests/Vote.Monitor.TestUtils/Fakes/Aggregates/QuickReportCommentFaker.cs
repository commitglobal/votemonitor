using Vote.Monitor.Domain.Entities.QuickReportCommentAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class QuickReportCommentFaker : PrivateFaker<QuickReportComment>
{
    public QuickReportCommentFaker(Guid? id = null,
        string? text = null,
        Guid? electionRoundId = null,
        Guid? quickReportId = null,
        Guid? authorId = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Text, text ?? "a quick report comment");
        RuleFor(fake => fake.ElectionRoundId, fake => electionRoundId ?? fake.Random.Guid());
        RuleFor(fake => fake.QuickReportId, fake => quickReportId ?? fake.Random.Guid());
        RuleFor(fake => fake.CreatedBy, fake => authorId ?? fake.Random.Guid());
    }
}
