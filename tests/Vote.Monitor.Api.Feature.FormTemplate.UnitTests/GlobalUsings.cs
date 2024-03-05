global using FastEndpoints;
global using FluentAssertions;
global using FluentValidation.TestHelper;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.Extensions.Logging.Abstractions;
global using NSubstitute;
global using Vote.Monitor.Api.Feature.FormTemplate.Update.Requests;
global using Vote.Monitor.Api.Feature.FormTemplate.Update.Validators;
global using Xunit;

global using NgoAggregate = Vote.Monitor.Domain.Entities.NgoAggregate.Ngo;
global using Vote.Monitor.Core.Models;
global using Vote.Monitor.Core.Services.Time;
global using Vote.Monitor.Domain.Constants;
global using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
global using Vote.Monitor.Domain.Entities.FormTemplateAggregate.Questions;
global using Vote.Monitor.Domain.Repository;
global using Vote.Monitor.TestUtils;
