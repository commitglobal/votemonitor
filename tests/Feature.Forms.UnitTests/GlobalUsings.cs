global using FastEndpoints;
global using Feature.Forms;
global using Feature.Forms.Specifications;
global using FluentAssertions;
global using FluentValidation.TestHelper;
global using Microsoft.AspNetCore.Http.HttpResults;
global using NSubstitute;
global using Vote.Monitor.Core.Models;
global using Xunit;

global using FormTemplateAggregate = Vote.Monitor.Domain.Entities.FormTemplateAggregate.FormTemplate;
global using Vote.Monitor.Core.Services.Time;
global using Vote.Monitor.Domain.Constants;
global using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
global using Vote.Monitor.Domain.Repository;
global using Vote.Monitor.TestUtils;
global using Vote.Monitor.TestUtils.Fakes.Aggregates;
