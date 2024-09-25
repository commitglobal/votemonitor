﻿namespace Feature.CitizenReports.Notes.Upsert;

public class Request
{
    public Guid ElectionRoundId { get; set; }
    public Guid CitizenReportId { get; set; }
    public Guid FormId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
}
