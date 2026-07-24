using System.Security.Claims;
using OrderService.Controllers;
using Xunit;

namespace CatalogService.Tests;

public class OrderClaimsTests
{
    [Fact]
    public void TryGetCustomerId_ReturnsGuidFromNameIdentifierClaim()
    {
        var customerId = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, customerId.ToString())
        }, "Test"));

        var result = OrdersController.TryGetCustomerId(user, out var parsedCustomerId);

        Assert.True(result);
        Assert.Equal(customerId, parsedCustomerId);
    }
}
