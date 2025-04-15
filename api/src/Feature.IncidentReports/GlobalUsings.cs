// Global using directives

global using System.Runtime.CompilerServices;
global using Ardalis.Specification;
global using Authorization.Policies;
global using Authorization.Policies.Requirements;
global using FastEndpoints;
global using Feature.IncidentReports.Models;
global using FluentValidation;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Module.Answers.Mappers;
global using Vote.Monitor.Core.Models;
global using Vote.Monitor.Core.Services.FileStorage.Contracts;
global using Vote.Monitor.Domain;
global using Vote.Monitor.Domain.Repository;
global using Module.Forms.Mappers;
global using IncidentReportAggregate = Vote.Monitor.Domain.Entities.IncidentReportAggregate.IncidentReport;
global using FormAggregate = Vote.Monitor.Domain.Entities.FormAggregate.Form;
global using PollingStationAggregate = Vote.Monitor.Domain.Entities.PollingStationAggregate.PollingStation;