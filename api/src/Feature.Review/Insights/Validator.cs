namespace Feature.Review.Insights;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.ElectionRoundId).NotEmpty();
        RuleFor(x => x.NgoId).NotEmpty();
        RuleFor(x => x.RecordType).NotNull();
        RuleFor(x => x.DataSource).NotNull();
        RuleFor(x => x.XAxis)
            .NotEmpty()
            .Must((req, axis) => req.RecordType is null || InsightAxes.For(req.RecordType).ContainsKey(axis))
            .WithMessage("Unknown axis for this record type.");
        RuleFor(x => x.GroupBy)
            .Must((req, axis) => req.RecordType is null || InsightAxes.For(req.RecordType).ContainsKey(axis!))
            .WithMessage("Unknown axis for this record type.")
            .NotEqual(x => x.XAxis)
            .When(x => x.GroupBy is not null);
        RuleFor(x => x.Metric)
            .NotEmpty()
            .Must((req, metric) => req.RecordType is null || InsightMetrics.For(req.RecordType).ContainsKey(metric))
            .WithMessage("Unknown metric for this record type.");
        RuleFor(x => x.TimeZone)
            .NotEmpty()
            .Must(zone => TimeZoneInfo.TryFindSystemTimeZoneById(zone, out _))
            .WithMessage("Unknown time zone.");
    }
}
