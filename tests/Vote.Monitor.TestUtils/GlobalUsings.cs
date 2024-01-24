// Global using directives

global using System.Text.Json.Serialization;
global using Ardalis.SmartEnum.SystemTextJson;
global using Ardalis.Specification;
global using FastEndpoints;
global using FluentValidation;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Vote.Monitor.Core.Models;
global using Vote.Monitor.Domain.Entities.ElectionRoundAggregate;
global using Vote.Monitor.Domain.Repository;
global using Microsoft.AspNetCore.Http;
global using Vote.Monitor.Core.Services.Time;
global using Vote.Monitor.Domain.Constants;
global using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
global using ElectionRoundAggregate = Vote.Monitor.Domain.Entities.ElectionRoundAggregate.ElectionRound;
global using NgoAggregate = Vote.Monitor.Domain.Entities.CSOAggregate.CSO;
global using ObserverAggregate = Vote.Monitor.Domain.Entities.ApplicationUserAggregate.Observer;
