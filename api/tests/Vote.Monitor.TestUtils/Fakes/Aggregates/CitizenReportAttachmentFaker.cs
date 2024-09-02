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

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeCitizenReport = citizenReport ?? new CitizenReportFaker(electionRound: fakeElectionRound).Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.FileName, fileName ?? string.Empty);
        RuleFor(fake => fake.FilePath, filePath ?? string.Empty);
        RuleFor(fake => fake.ElectionRound, electionRound ?? fakeElectionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound?.Id ?? fakeElectionRound.Id);
        RuleFor(fake => fake.CitizenReport, fakeCitizenReport ?? fakeCitizenReport);
        RuleFor(fake => fake.CitizenReportId, fakeCitizenReport?.Id ?? fakeCitizenReport.Id);
    }
}
