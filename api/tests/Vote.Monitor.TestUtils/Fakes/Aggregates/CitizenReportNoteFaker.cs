using Vote.Monitor.Domain.Entities.CitizenReportAggregate;
using Vote.Monitor.Domain.Entities.CitizenReportNoteAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class CitizenReportNoteFaker : PrivateFaker<CitizenReportNote>
{
    public CitizenReportNoteFaker(Guid? id = null,
        string? text = null,
        ElectionRound? electionRound = null,
        CitizenReport? citizenReport = null)
    {
        UsePrivateConstructor();

        var fakeElectionRound = new ElectionRoundAggregateFaker().Generate();
        var fakeCitizenReport = new CitizenReportFaker(electionRound: fakeElectionRound).Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Text, text ?? string.Empty);
        RuleFor(fake => fake.ElectionRound, electionRound ?? fakeElectionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound?.Id ?? fakeElectionRound.Id);
        RuleFor(fake => fake.CitizenReport, fakeCitizenReport ?? fakeCitizenReport);
        RuleFor(fake => fake.CitizenReportId, fakeCitizenReport?.Id ?? fakeCitizenReport.Id);
    }
}