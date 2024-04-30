//using System.Security.Claims;
//using Microsoft.AspNetCore.Authorization;
//using Vote.Monitor.Domain.Entities.FormAggregate;

//namespace Feature.QuickReports.UnitTests.Endpoints;

//public class ListEndpointTests
//{
//    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
//    private readonly IReadRepository<Form> _repository = Substitute.For<IReadRepository<Form>>();
//    private readonly List.Endpoint _endpoint;

//    public ListEndpointTests()
//    {
//        _endpoint = Factory.Create<List.Endpoint>(_authorizationService, _repository);
//        _authorizationService
//            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
//                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());
//    }

//    [Fact]
//    public async Task Should_UseCorrectSpecification()
//    {
//        // Arrange
//        _repository
//            .ListAsync(Arg.Any<ListFormsSpecification>())
//            .Returns([]);

//        // Act
//        var request = new List.Request();
//        var result = await _endpoint.ExecuteAsync(request, default);

//        // Assert
//        result
//            .Should()
//            .BeOfType<Results<Ok<PagedResponse<FormSlimModel>>, NotFound>>();

//        await _repository.Received(1).ListAsync(Arg.Any<ListFormsSpecification>());
//        await _repository.Received(1).CountAsync(Arg.Any<ListFormsSpecification>());
//    }

//    [Fact]
//    public async Task Should_ReturnMappedForms()
//    {
//        // Arrange
//        var numberOfForms = 3;
//        var totalCount = 154;
//        var pageSize = 100;

//        var forms = new FormSlimModelFaker().Generate(numberOfForms);

//        _repository
//            .ListAsync(Arg.Any<ListFormsSpecification>())
//            .Returns(forms);

//        _repository
//            .CountAsync(Arg.Any<ListFormsSpecification>())
//            .Returns(totalCount);

//        // Act
//        var request = new List.Request
//        {
//            PageSize = pageSize,
//            PageNumber = numberOfForms
//        };
//        var result = await _endpoint.ExecuteAsync(request, default);

//        // Assert
//        result
//            .Should().BeOfType<Results<Ok<PagedResponse<FormSlimModel>>, NotFound>>()
//            .Which
//            .Result.Should().BeOfType<Ok<PagedResponse<FormSlimModel>>>()
//            .Which.Value.Should().NotBeNull();

//        var pagedResult = (result.Result as Ok<PagedResponse<FormSlimModel>>);

//        pagedResult.Value.PageSize.Should().Be(pageSize);
//        pagedResult.Value.CurrentPage.Should().Be(numberOfForms);
//        pagedResult.Value.TotalCount.Should().Be(totalCount);
//        pagedResult.Value.Items.Should().BeEquivalentTo(forms);
//    }
//}
