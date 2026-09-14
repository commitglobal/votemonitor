using Vote.Monitor.Domain.Entities.CitizenReportCommentAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class CitizenReportCommentFaker : PrivateFaker<CitizenReportComment>
{
    public CitizenReportCommentFaker(Guid? id = null,
        string? text = null,
        Guid? electionRoundId = null,
        Guid? citizenReportId = null,
        Guid? authorId = null)
    {
        UsePrivateConstructor();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Text, text ?? "a citizen report comment");
        RuleFor(fake => fake.ElectionRoundId, fake => electionRoundId ?? fake.Random.Guid());
        RuleFor(fake => fake.CitizenReportId, fake => citizenReportId ?? fake.Random.Guid());
        RuleFor(fake => fake.CreatedBy, fake => authorId ?? fake.Random.Guid());
    }
}
