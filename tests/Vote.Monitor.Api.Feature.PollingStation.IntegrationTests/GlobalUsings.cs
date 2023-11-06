// Global using directives

global using System.Net;
global using Bogus;
global using FastEndpoints.Testing;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.TestHost;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Testcontainers.PostgreSql;
global using Vote.Monitor.Api;
global using Vote.Monitor.Domain;
global using Xunit.Abstractions;
global using FastEndpoints;
global using FastEndpoints.Testing;
global using FluentAssertions;
global using Xunit;
global using Xunit.Abstractions;
global using Xunit.Priority;

using Create = Vote.Monitor.Api.Feature.PollingStation.Create;
using Get = Vote.Monitor.Api.Feature.PollingStation.Get;
using Delete = Vote.Monitor.Api.Feature.PollingStation.Delete;
using Update = Vote.Monitor.Api.Feature.PollingStation.Update;
using Import = Vote.Monitor.Api.Feature.PollingStation.Import;
using GetTagValues = Vote.Monitor.Api.Feature.PollingStation.GetTagValues;
using List = Vote.Monitor.Api.Feature.PollingStation.List;
