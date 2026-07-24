using System.IdentityModel.Tokens.Jwt; using System.Security.Claims; using CatalogService.Data; using CatalogService.Models; using OrderService.Contracts; using OrderService.Data; using OrderService.Models; using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace OrderService.Controllers;
[ApiController, Route("api/orders"), Authorize]
public sealed class OrdersController(OrderDbContext db, CatalogDbContext catalogDb) : ControllerBase
{
 [HttpPost] public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request, CancellationToken ct) { if (!TryGetCustomerId(User, out var customerId)) return Unauthorized();

    foreach (var line in request.Lines)
    {
        var product = await catalogDb.Products.SingleOrDefaultAsync(x => x.Slug == line.ProductId, ct);
        if (product is null) return BadRequest(new { message = $"El producto '{line.ProductId}' no existe." });
        if (product.Existencia < line.Quantity) return Conflict(new { message = $"No hay existencia suficiente para '{product.Name}'." });
        product.Existencia -= line.Quantity;
    }

    var lines = request.Lines.Select(x => new OrderLine { ProductId=x.ProductId, Quantity=x.Quantity, UnitPrice=x.UnitPrice }).ToList();
    var order = new Order { CustomerId=customerId, Lines=lines, Total=lines.Sum(x => x.UnitPrice*x.Quantity) }; db.Orders.Add(order); await catalogDb.SaveChangesAsync(ct); await db.SaveChangesAsync(ct); return CreatedAtAction(nameof(Status), new { number=order.Number }, new OrderResponse(order.Number, order.Status, order.Total)); }
 [AllowAnonymous, HttpGet("{number}/status")] public async Task<ActionResult<OrderResponse>> Status(string number, CancellationToken ct) { var order = await db.Orders.AsNoTracking().SingleOrDefaultAsync(x => x.Number == number, ct); return order is null ? NotFound() : Ok(new OrderResponse(order.Number, order.Status, order.Total)); }

 public static bool TryGetCustomerId(ClaimsPrincipal user, out Guid customerId)
 {
     foreach (var claimType in new[] { JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier })
     {
         var claimValue = user.FindFirst(claimType)?.Value;
         if (Guid.TryParse(claimValue, out customerId)) return true;
     }

     customerId = default;
     return false;
 }
}
