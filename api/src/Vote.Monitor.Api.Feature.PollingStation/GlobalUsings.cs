// Global using directives

global using System.Text.Json;
global using Ardalis.Specification;
global using CsvHelper;
global using EFCore.BulkExtensions;
global using FastEndpoints;
global using FluentValidation;
global using FluentValidation.Results;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Vote.Monitor.Core.Services.Csv;
global using Vote.Monitor.Domain;
global using Vote.Monitor.Domain.Repository;
global using PollingStationAggregate = Vote.Monitor.Domain.Entities.PollingStationAggregate.PollingStation;
global using ElectionRoundAggregate = Vote.Monitor.Domain.Entities.ElectionRoundAggregate.ElectionRound;
