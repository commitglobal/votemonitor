using System.Text.Json;
using Authorization.Policies;
using Feature.DataExport.Specifications;
using Module.Forms.Mappers;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
using Vote.Monitor.Domain.Entities.FormAggregate;
using Vote.Monitor.Domain.Entities.ObserverGuideAggregate;
using Vote.Monitor.Domain.Entities.PollingStationAggregate;

namespace Feature.DataExport.Export;

public class Endpoint(
    IReadRepository<ElectionRound> electionRoundRepository,
    IReadRepository<PollingStation> pollingStationRepository,
    IReadRepository<ObserverGuide> guideRepository,
    IReadRepository<Form> formRepository
) : Endpoint<Request, Results<Ok<Response>, NotFound>>
{
    public override void Configure()
    {
        Get("/api/election-rounds/{electionRoundId}/export");
        DontAutoTag();
        Options(x => x.WithTags("exported-data"));
        Summary(s =>
        {
            s.Summary = "Export ngo data for VoteMonitor Zero";
        });
        Policies(PolicyNames.NgoAdminsOnly);
    }

    public override async Task<Results<Ok<Response>, NotFound>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var electionRound =
            await electionRoundRepository.FirstOrDefaultAsync(new GetElectionRoundSpecification(req.ElectionRoundId),
                ct);
        if (electionRound == null)
        {
            return TypedResults.NotFound();
        }

        var forms = await formRepository.ListAsync(new ListFormsSpecification(req.ElectionRoundId, req.NgoId), ct);
        var guides = await guideRepository.ListAsync(new ListGuidesSpecification(req.ElectionRoundId, req.NgoId), ct);
        var pollingStations =
            await pollingStationRepository.ListAsync(new ListPollingStationsSpecification(req.ElectionRoundId), ct);

        var mappedForms = forms.Select(form => new Response.FormModel()
            {
                Id = form.Id,
                Code = form.Code,
                FormType = form.FormType,
                DefaultLanguage = form.DefaultLanguage,
                Languages = form.Languages,
                Name = form.Name,
                Questions = form.Questions.Select(QuestionsMapper.ToModel).ToList(),
                Description = form.Description,
                Icon = form.Icon,
                DisplayOrder = form.DisplayOrder
            })
            .ToList();
        var response = new Response
        {
            ElectionRound = electionRound,
            Forms = mappedForms,
            Guides = guides,
            PollingStations = pollingStations.ToPollingStationNodes(),
            // Base64 = JsonSerializer.Serialize(new
            // {
            //     ElectionRound = electionRound, Forms = mappedForms,
            //     // Guides = guides,
            //     PollingStations = pollingStations,
            // }, new JsonSerializerOptions(JsonSerializerDefaults.Web))
            // .ToBase64String()
        };
        
        
        
        return TypedResults.Ok(response);
    }
}
