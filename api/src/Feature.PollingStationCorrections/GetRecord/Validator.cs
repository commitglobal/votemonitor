namespace Feature.PollingStationCorrections.GetRecord;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.RecordType).NotNull();
        RuleFor(x => x.RecordId).NotEmpty();
    }
}
