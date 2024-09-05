global using FastEndpoints;
global using FluentAssertions;
global using FluentValidation.TestHelper;
global using Microsoft.AspNetCore.Http.HttpResults;
global using NSubstitute;
global using Xunit;

global using FormTemplateAggregate = Vote.Monitor.Domain.Entities.FormTemplateAggregate.FormTemplate;
global using Vote.Monitor.Domain.Entities.FormTemplateAggregate;
global using Vote.Monitor.Domain.Repository;
global using Vote.Monitor.TestUtils;
global using Vote.Monitor.TestUtils.Fakes.Aggregates;
