// Global using directives

global using System.Text.Json.Serialization;
global using Ardalis.SmartEnum.SystemTextJson;
global using Ardalis.Specification;
global using FastEndpoints;
global using FluentValidation;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Feature.Observers.Specifications;
global using Vote.Monitor.Domain.Entities.ApplicationUserAggregate;
global using Vote.Monitor.Domain.Repository;
global using ObserverAggregate = Vote.Monitor.Domain.Entities.ObserverAggregate.Observer;
global using ImportValidationErrors = Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate.ImportValidationErrors;
global using Authorization.Policies;
global using Microsoft.Extensions.Logging;
global using Vote.Monitor.Core.Models;
global using Vote.Monitor.Domain.Entities.ImportValidationErrorsAggregate;
