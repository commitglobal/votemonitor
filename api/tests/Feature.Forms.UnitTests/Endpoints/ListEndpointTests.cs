using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Vote.Monitor.Domain.Entities.FormAggregate;

namespace Feature.Forms.UnitTests.Endpoints;

public class ListEndpointTests
{
    private readonly IAuthorizationService _authorizationService = Substitute.For<IAuthorizationService>();
    private readonly List.Endpoint _endpoint;

    public ListEndpointTests()
    {
        _endpoint = Factory.Create<List.Endpoint>(_authorizationService);
        _authorizationService
            .AuthorizeAsync(Arg.Any<ClaimsPrincipal>(), Arg.Any<object>(),
                Arg.Any<IEnumerable<IAuthorizationRequirement>>()).Returns(AuthorizationResult.Success());
    }
    
    
    
    
    
    

   
}
