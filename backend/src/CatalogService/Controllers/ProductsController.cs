using CatalogService.Contracts; using CatalogService.Data; using Microsoft.AspNetCore.Mvc; using Microsoft.EntityFrameworkCore;
namespace CatalogService.Controllers;
[ApiController, Route("api/products")]
public sealed class ProductsController(CatalogDbContext db) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetAll(CancellationToken ct) => Ok(await db.Products.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).Select(x => new ProductResponse(x.Slug, x.Name, x.Price, x.Existencia, x.Category, x.ImageUrl, x.Description)).ToListAsync(ct));
    [HttpGet("{slug}")] public async Task<ActionResult<ProductResponse>> Get(string slug, CancellationToken ct) { var product = await db.Products.AsNoTracking().Where(x => x.IsActive && x.Slug == slug).Select(x => new ProductResponse(x.Slug, x.Name, x.Price, x.Existencia, x.Category, x.ImageUrl, x.Description)).SingleOrDefaultAsync(ct); return product is null ? NotFound() : Ok(product); }

    [HttpPatch("{slug}/stock")]
    public async Task<ActionResult<ProductResponse>> UpdateStock(string slug, SetStockRequest request, CancellationToken ct)
    {
        if (request.Existencia < 0) return BadRequest(new { message = "La existencia no puede ser negativa." });

        var product = await db.Products.SingleOrDefaultAsync(x => x.Slug == slug, ct);
        if (product is null) return NotFound();

        product.Existencia = request.Existencia;
        await db.SaveChangesAsync(ct);

        return Ok(new ProductResponse(product.Slug, product.Name, product.Price, product.Existencia, product.Category, product.ImageUrl, product.Description));
    }
}
