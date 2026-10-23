namespace Feature.Review.Metrics;

public record ReviewMetricsModel(RecordTypeMetricsModel[] RecordTypes);

public record RecordTypeMetricsModel(
    ReportRecordType RecordType,
    int Total,
    StageCountModel[] Stages,
    SeverityCountModel[] Severities,
    PlaceMetricsModel[]? Places);

public record StageCountModel(Guid StageId, int Count);

public record SeverityCountModel(string Severity, int Count);

public record PlaceMetricsModel(
    string? Level1,
    string? Level2,
    string? Level3,
    string? Level4,
    string? Level5,
    int Total,
    StageCountModel[] Stages,
    SeverityCountModel[] Severities);
