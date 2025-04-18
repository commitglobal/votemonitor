﻿using Vote.Monitor.Core.Models;
using Vote.Monitor.Core.Security;
using Vote.Monitor.Domain.Entities.FormBase;
using Module.Forms.Requests;

namespace Feature.Forms.Create;

public class Request
{
    public Guid ElectionRoundId { get; set; }

    [FromClaim(ApplicationClaimTypes.NgoId)]
    public Guid NgoId { get; set; }

    public string Code { get; set; }
    public TranslatedString Name { get; set; } = new();
    public TranslatedString Description { get; set; } = new();
    public FormType FormType { get; set; }
    public List<string> Languages { get; set; } = [];
    public string DefaultLanguage { get; set; }
    public string? Icon { get; set; }

    public List<BaseQuestionRequest> Questions { get; set; } = new();
}
