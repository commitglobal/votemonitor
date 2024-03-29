﻿using Vote.Monitor.Answer.Module.Mappers;
using Vote.Monitor.Answer.Module.Models;
using Vote.Monitor.Domain.Entities.PollingStationInfoAggregate;

namespace Vote.Monitor.Api.Feature.PollingStation.Information;

public record PollingStationInformationModel
{
    public required Guid Id { get; init; }

    public required DateTime? ArrivalTime { get; set; }
    public required DateTime? DepartureTime { get; set; }
    public IReadOnlyList<BaseAnswerModel> Answers { get; set; }

    public static PollingStationInformationModel FromEntity(PollingStationInformation entity) => new()
    {
        Id = entity.Id,
        Answers = entity.Answers
            .Select(AnswerMapper.ToModel)
            .ToList(),

        ArrivalTime = entity.ArrivalTime,
        DepartureTime = entity.DepartureTime
    };
}
