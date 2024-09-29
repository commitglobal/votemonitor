using Vote.Monitor.Domain.Entities.IssueReportAggregate;
using Vote.Monitor.Domain.Entities.IssueReportNoteAggregate;

namespace Vote.Monitor.TestUtils.Fakes.Aggregates;

public sealed class IssueReportNoteFaker : PrivateFaker<IssueReportNote>
{
    public IssueReportNoteFaker(Guid? id = null,
        string? text = null,
        ElectionRound? electionRound = null,
        IssueReport? issueReport = null)
    {
        UsePrivateConstructor();

        electionRound ??= new ElectionRoundAggregateFaker().Generate();
        issueReport ??= new IssueReportFaker(electionRound: electionRound).Generate();

        RuleFor(fake => fake.Id, fake => id ?? fake.Random.Guid());
        RuleFor(fake => fake.Text, text ?? string.Empty);
        RuleFor(fake => fake.ElectionRound, electionRound);
        RuleFor(fake => fake.ElectionRoundId, electionRound.Id);
        RuleFor(fake => fake.IssueReport, issueReport);
        RuleFor(fake => fake.IssueReportId, issueReport.Id);
    }
}