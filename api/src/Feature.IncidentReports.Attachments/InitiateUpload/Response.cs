﻿namespace Feature.IncidentReports.Attachments.InitiateUpload;

public class Response
{
    public string UploadId { get; set; }
    public Dictionary<int, string> UploadUrls { get; set; }
}
