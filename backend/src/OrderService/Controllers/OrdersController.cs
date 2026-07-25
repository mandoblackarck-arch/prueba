using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CatalogService.Data;
using OrderService.Contracts;
using OrderService.Data;
using OrderService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Controllers;

[ApiController, Route("api/orders"), Authorize]
public sealed class OrdersController(OrderDbContext db, CatalogDbContext catalogDb, ILogger<OrdersController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create(CreateOrderRequest request, CancellationToken ct)
    {
        if (!TryGetCustomerId(User, out var customerId))
        {
            logger.LogWarning("Order creation rejected because the authenticated user has no valid customer identifier. TraceId: {TraceId}", HttpContext.TraceIdentifier);
            return Unauthorized();
        }

        logger.LogInformation("Creating order for customer {CustomerId} with {LineCount} line(s). TraceId: {TraceId}", customerId, request.Lines.Count, HttpContext.TraceIdentifier);
        try
        {
            foreach (var line in request.Lines)
            {
                var product = await catalogDb.Products.SingleOrDefaultAsync(x => x.Slug == line.ProductId, ct);
                if (product is null)
                {
                    logger.LogWarning("Order creation rejected: product {ProductId} does not exist. TraceId: {TraceId}", line.ProductId, HttpContext.TraceIdentifier);
                    return BadRequest(new { message = $"El producto '{line.ProductId}' no existe." });
                }

                if (product.Existencia < line.Quantity)
                {
                    logger.LogWarning("Order creation rejected: insufficient stock for product {ProductId}. Requested {RequestedQuantity}; Available {AvailableQuantity}. TraceId: {TraceId}", line.ProductId, line.Quantity, product.Existencia, HttpContext.TraceIdentifier);
                    return Conflict(new { message = $"No hay existencia suficiente para '{product.Name}'." });
                }

                product.Existencia -= line.Quantity;
            }

            var lines = request.Lines.Select(x => new OrderLine { ProductId = x.ProductId, Quantity = x.Quantity, UnitPrice = x.UnitPrice }).ToList();
            var order = new Order { CustomerId = customerId, Lines = lines, Total = lines.Sum(x => x.UnitPrice * x.Quantity) };
            db.Orders.Add(order);
            await catalogDb.SaveChangesAsync(ct);
            await db.SaveChangesAsync(ct);

            logger.LogInformation("Order {OrderNumber} created for customer {CustomerId}. Total: {Total}. TraceId: {TraceId}", order.Number, customerId, order.Total, HttpContext.TraceIdentifier);
            return CreatedAtAction(nameof(Status), new { number = order.Number }, new OrderResponse(order.Number, order.Status, order.Total));
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Database error while creating an order for customer {CustomerId}. TraceId: {TraceId}", customerId, HttpContext.TraceIdentifier);
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "No fue posible guardar el pedido. Consulte el TraceId en los logs.");
        }
    }

    [AllowAnonymous, HttpGet("{number}/status")]
    public async Task<ActionResult<OrderResponse>> Status(string number, CancellationToken ct)
    {
        logger.LogInformation("Looking up order status for {OrderNumber}. TraceId: {TraceId}", number, HttpContext.TraceIdentifier);
        var order = await db.Orders.AsNoTracking().SingleOrDefaultAsync(x => x.Number == number, ct);
        if (order is null)
        {
            logger.LogWarning("Order {OrderNumber} was not found. TraceId: {TraceId}", number, HttpContext.TraceIdentifier);
            return NotFound();
        }

        return Ok(new OrderResponse(order.Number, order.Status, order.Total));
    }

    private bool TryGetCustomerId(ClaimsPrincipal user, out Guid customerId)
    {
        logger.LogInformation(
            "Identifying customer. IsAuthenticated: {IsAuthenticated}; UserName: {UserName}; ClaimCount: {ClaimCount}; TraceId: {TraceId}",
            user.Identity?.IsAuthenticated ?? false,
            user.Identity?.Name,
            user.Claims.Count(),
            HttpContext.TraceIdentifier);

        foreach (var claimType in new[] { JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier })
        {
            var claimValue = user.FindFirst(claimType)?.Value;
            if (Guid.TryParse(claimValue, out customerId))
            {
                logger.LogInformation("Customer identified from claim type {ClaimType}. CustomerId: {CustomerId}; TraceId: {TraceId}", claimType, customerId, HttpContext.TraceIdentifier);
                return true;
            }

            logger.LogWarning("Customer identifier claim is absent or invalid. ClaimType: {ClaimType}; TraceId: {TraceId}", claimType, HttpContext.TraceIdentifier);
        }

        customerId = default;
        logger.LogWarning("Unable to identify customer from JWT claims. TraceId: {TraceId}", HttpContext.TraceIdentifier);
        return false;
    }
}
