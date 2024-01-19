global using FastEndpoints;
global using FluentAssertions;
global using FluentValidation.TestHelper;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.Extensions.Logging.Abstractions;
global using NSubstitute;
global using Xunit;

global using CSOAggregate = Vote.Monitor.Domain.Entities.CSOAggregate.CSO;
global using Vote.Monitor.Api.Feature.CSO.Specifications;
global using Vote.Monitor.Core.Models;
global using Vote.Monitor.Core.Services.Time;
global using Vote.Monitor.Domain.Entities.CSOAggregate;
global using Vote.Monitor.Domain.Repository;
global using Vote.Monitor.TestUtils;
