using Module.Forms.Models;
using Vote.Monitor.Core.Models;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.FormBase;

namespace Feature.DataExport.Export;

public class Response
{
    public ElectionRoundModel ElectionRound { get; set; }
    public List<GuideModel> Guides { get; set; } = new();
    public List<FormModel> Forms { get; set; } = new();
    public List<LocationNode> PollingStations { get; set; } = new();
    public string Base64 { get; set; }


    public class ElectionRoundModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string EnglishTitle { get; set; }
        public DateOnly StartDate { get; set; }
    }   
    
    public class FormModel
    {

        public required Guid Id { get; init; }

        public required FormType FormType { get; init; }

        public required string Code { get; init; }
        public TranslatedString Name { get; init; }

        public TranslatedString Description { get; init; }

        public required string DefaultLanguage { get; init; }
        public required string[] Languages { get; init; } = [];
        public string? Icon { get; init; }
        public IReadOnlyList<BaseQuestionModel> Questions { get; init; } = [];


        public int DisplayOrder { get; init; }
    }    
    public class GuideModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
