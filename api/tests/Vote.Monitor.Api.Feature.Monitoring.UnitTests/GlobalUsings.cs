global using FastEndpoints;
global using FluentAssertions;
global using FluentValidation.TestHelper;
global using Microsoft.AspNetCore.Http.HttpResults;
global using NSubstitute;
global using Vote.Monitor.Domain.Repository;
global using Vote.Monitor.Api.Feature.Monitoring.Specifications;
global using Vote.Monitor.TestUtils.Fakes.Aggregates;
global using Xunit;

global using ElectionRoundAggregate = Vote.Monitor.Domain.Entities.ElectionRoundAggregate.ElectionRound;
global using NgoAggregate = Vote.Monitor.Domain.Entities.NgoAggregate.Ngo;
global using MonitoringNgoAggregate = Vote.Monitor.Domain.Entities.MonitoringNgoAggregate.MonitoringNgo;

