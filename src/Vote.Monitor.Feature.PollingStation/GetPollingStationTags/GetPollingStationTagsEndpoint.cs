using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Vote.Monitor.Feature.PollingStation.Models;
using Vote.Monitor.Feature.PollingStation.Repositories;

namespace Vote.Monitor.Feature.PollingStation.GetPollingStationTags;
internal class GetPollingStationTagsEndpoint : EndpointWithoutRequest
{
    private readonly IRepository<PollingStationModel> _repository;
    public GetPollingStationTagsEndpoint(IRepository<PollingStationModel> repository)
    {
        _repository = repository;
    }

    public override void Configure()
    {
        Get("/api/polling-stations/tags");

        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
       var tags = await _repository.GetTags();

       await SendAsync(tags, cancellation: ct);
    }
}
