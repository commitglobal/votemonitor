using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Api.Feature.PollingStation.Information.Specifications;
using Vote.Monitor.Core.Extensions;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;
using Vote.Monitor.Domain.Entities.PollingStationInfoFormAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information.Update;

public class Endpoint(IRepository<PollingStationInformation> repository,
    IReadRepository<PollingStationInformationForm> formRepository) : Endpoint<Request, Results<Ok<PollingStationInformationModel>, NotFound, ValidationProblem>>
{
    public override void Configure()
    {
        Post("/api/election-rounds/{electionRoundId}/polling-stations/{pollingStationId}/information/{id}");
        DontAutoTag();
        Options(x => x.WithTags("polling-station-information"));
    }

    public override async Task<Results<Ok<PollingStationInformationModel>, NotFound, ValidationProblem>> ExecuteAsync(Request req, CancellationToken ct)
    {
        var formSpecification = new GetPollingStationInformationFormSpecification(req.ElectionRoundId, req.FormId);
        var form = await formRepository.FirstOrDefaultAsync(formSpecification, ct);
        if (form is null)
        {
            return TypedResults.NotFound();
        }

        var specification = new GetPollingStationInformationByIdSpecification(req.ElectionRoundId, req.PollingStationId, req.ObserverId, req.Id);
        var pollingStationInformation = await repository.FirstOrDefaultAsync(specification, ct);

        if (pollingStationInformation is null)
        {
            return TypedResults.NotFound();
        }

        var result = form.FillIn(pollingStationInformation, req.Answers.Select(AnswerMapper.ToEntity).ToList());

        if (result is FillInPollingStationInformationResult.ValidationFailed error)
        {
            return TypedResults.ValidationProblem(error.ValidationResult.Errors.ToValidationErrorDictionary());
        }

        var success = (result as FillInPollingStationInformationResult.Ok)!;

        return TypedResults.Ok(PollingStationInformationModel.FromEntity(success.PollingStationInformation));
    }
}
