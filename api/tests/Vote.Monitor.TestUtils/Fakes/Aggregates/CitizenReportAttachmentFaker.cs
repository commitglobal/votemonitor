using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.CitizenReportAttachmentAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class CitizenReportAttachmentFaker : PrivateFaker<CitizenReportAttachment>
{
    public CitizenReportAttachmentFaker(Guid? id = null,
        string? fileName = null,
        string? filePath = null,
        ElectionRound? electionRound = null,
        CitizenReport? citizenReport = null)
    {
        UsePrivateConstructor();

        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        citizenReport ??= new CitizenReportFaker(electionRound: electionRound).Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.FileName, fileName ?? string.Empty);
        RuleFor(fake => fake.FilePath, filePath ?? string.Empty);
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        RuleFor(fake => fake.CitizenReport, citizenReport);
        RuleFor(fake => fake.CitizenReportId, citizenReport.Id);
    }
}