using Vote.Monitor.TestUtils.Fakes;

namespace Feature.CitizenReports.Notes.UnitTests.Endpoints;

public sealed class CitizenReportNoteModelFaker: PrivateFaker<CitizenReportNoteModel>
{
    public CitizenReportNoteModelFaker()
    {
        RuleFor(fake => fake.Id, fake =>  fake.Random.Guid());
        RuleFor(fake => fake.Text, fake => fake.Lorem.Sentence());
        RuleFor(fake => fake.ElectionRoundId, fake=>fake.Random.Guid());
        RuleFor(fake => fake.CitizenReportId, fake=>fake.Random.Guid());
        RuleFor(fake => fake.FormId, fake=>fake.Random.Guid());
        RuleFor(fake => fake.QuestionId, fake=>fake.Random.Guid());
        RuleFor(fake => fake.CreatedAt, fake=>fake.Date.Recent());
        RuleFor(fake => fake.UpdatedAt, fake=>fake.Date.Recent());
    }
}