// Global using directives

global using System.Text.Json;
global using Bogus;
global using FluentAssertions;
global using FluentValidation;
global using FluentValidation.TestHelper;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.Options;
global using NSubstitute;
global using Vote.Monitor.Core.Services.Csv;
global using Feature.PollingStations.Options;
global using Feature.PollingStations.Services;
global using Feature.PollingStations.Specifications;
global using Vote.Monitor.TestUtils;
global using Xunit;
global using PollingStationAggregate = Vote.Monitor.Domain.Entities.PollingStationAggregate.PollingStation;
