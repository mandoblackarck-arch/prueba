using CatalogService.Models;
namespace CatalogService.Tests;
public sealed class ProductTests { [Fact] public void New_product_is_active() { var product = new Product(); Assert.True(product.IsActive); Assert.NotEqual(Guid.Empty, product.Id); } }
