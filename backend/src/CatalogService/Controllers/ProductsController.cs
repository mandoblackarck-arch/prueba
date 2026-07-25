using CatalogService.Contracts;
using CatalogService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Controllers;

[ApiController, Route("api/products")]
public sealed class ProductsController(CatalogDbContext db, ILogger<ProductsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(CancellationToken ct) => Ok(await db.Products.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).Select(x => new ProductResponse(x.Slug, x.Name, x.Price, x.Existencia, x.Category, x.ImageUrl, x.Description)).ToListAsync(ct));

    [HttpGet("{slug}")]
    public async Task<ActionResult<ProductResponse>> Get(string slug, CancellationToken ct)
    {
        var product = await db.Products.AsNoTracking().Where(x => x.IsActive && x.Slug == slug).Select(x => new ProductResponse(x.Slug, x.Name, x.Price, x.Existencia, x.Category, x.ImageUrl, x.Description)).SingleOrDefaultAsync(ct);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPatch("{slug}/stock")]
    public async Task<ActionResult<ProductResponse>> UpdateStock(string slug, SetStockRequest request, CancellationToken ct)
    {
        logger.LogInformation("Updating stock for product {ProductSlug} to {Stock}. TraceId: {TraceId}", slug, request.Existencia, HttpContext.TraceIdentifier);
        if (request.Existencia < 0)
        {
            logger.LogWarning("Stock update rejected for product {ProductSlug}: negative stock. TraceId: {TraceId}", slug, HttpContext.TraceIdentifier);
            return BadRequest(new { message = "La existencia no puede ser negativa." });
        }

        var product = await db.Products.SingleOrDefaultAsync(x => x.Slug == slug, ct);
        if (product is null)
        {
            logger.LogWarning("Stock update failed: product {ProductSlug} was not found. TraceId: {TraceId}", slug, HttpContext.TraceIdentifier);
            return NotFound();
        }

        try
        {
            product.Existencia = request.Existencia;
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Database error updating stock for product {ProductSlug}. TraceId: {TraceId}", slug, HttpContext.TraceIdentifier);
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "No fue posible actualizar la existencia. Consulte el TraceId en los logs.");
        }

        logger.LogInformation("Stock updated for product {ProductSlug}. TraceId: {TraceId}", slug, HttpContext.TraceIdentifier);
        return Ok(new ProductResponse(product.Slug, product.Name, product.Price, product.Existencia, product.Category, product.ImageUrl, product.Description));
    }
}
