// Global using directives

global using System.Text.Json;
global using Bogus;
global using FluentAssertions;
global using FluentValidation;
global using FluentValidation.TestHelper;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.Options;
global using NSubstitute;
global using Vote.Monitor.Api.Feature.PollingStation.Helpers;
global using Vote.Monitor.Api.Feature.PollingStation.List;
global using Vote.Monitor.Core.Services.Csv;
global using Vote.Monitor.Api.Feature.PollingStation.Options;
global using Vote.Monitor.Api.Feature.PollingStation.Services;
global using Vote.Monitor.Api.Feature.PollingStation.Specifications;
global using Vote.Monitor.Core.Models;
global using Vote.Monitor.TestUtils;
global using Xunit;
global using PollingStationAggregate = Vote.Monitor.Domain.Entities.PollingStationAggregate.PollingStation;
