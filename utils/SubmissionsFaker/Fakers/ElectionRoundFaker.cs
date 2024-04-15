using Bogus;
using SubmissionsFaker.Clients.PlatformAdmin;

namespace SubmissionsFaker.Fakers;

public sealed class ElectionRoundFaker : Faker<ElectionRound>
{
    public ElectionRoundFaker()
    {
        // Republic of Azerbaijan
        RuleFor(x => x.CountryId, new Guid("008c3138-73d8-dbbc-f1dd-521e4c68bcf1"));
        RuleFor(x => x.Title, f => f.Lorem.Sentence());
        RuleFor(x => x.EnglishTitle, f => f.Lorem.Sentence());
        RuleFor(x => x.StartDate, f => f.Date.FutureDateOnly());
    }
}