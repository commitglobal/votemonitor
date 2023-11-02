// Global using directives

global using System.Text.Json;
global using Ardalis.Specification;
global using FastEndpoints;
global using FluentValidation;
global using FluentValidation.Results;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using Vote.Monitor.Core.Helpers;
global using Vote.Monitor.Core.Services.Csv;
global using Vote.Monitor.Domain;
global using Vote.Monitor.Domain.Repository;
global using Vote.Monitor.Feature.PollingStation.Import;
global using Vote.Monitor.Feature.PollingStation.Options;
global using Vote.Monitor.Feature.PollingStation.Services;
global using Vote.Monitor.Feature.PollingStation.Specifications;
global using PollingStationAggregate = Vote.Monitor.Domain.Entities.PollingStationAggregate.PollingStation;
