using System.Net.Http.Json;
using CatalogService.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace CatalogService.Tests;

public sealed class CatalogApiIntegrationTests : IClassFixture<CatalogApiFactory>
{
    private readonly HttpClient _client;
    public CatalogApiIntegrationTests(CatalogApiFactory factory) => _client = factory.CreateClient();
    [Fact] public async Task Products_endpoint_returns_seeded_catalog() { var response = await _client.GetAsync("/api/products"); response.EnsureSuccessStatusCode(); Assert.Contains("vela-ambar", await response.Content.ReadAsStringAsync()); }

    [Fact]
    public async Task Products_endpoint_exposes_stock_quantity_and_updates_it()
    {
        var response = await _client.GetAsync("/api/products");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("stockQuantity", body);

        var updateResponse = await _client.PatchAsJsonAsync("/api/products/vela-ambar/stock", new { stockQuantity = 7 });
        updateResponse.EnsureSuccessStatusCode();

        var updatedResponse = await _client.GetAsync("/api/products");
        updatedResponse.EnsureSuccessStatusCode();
        Assert.Contains("\"stockQuantity\":7", await updatedResponse.Content.ReadAsStringAsync());
    }
}
public sealed class CatalogApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    protected override void ConfigureWebHost(IWebHostBuilder builder) { _connection.Open(); builder.ConfigureServices(services => { services.RemoveAll<DbContextOptions<CatalogDbContext>>(); services.AddDbContext<CatalogDbContext>(o => o.UseSqlite(_connection)); }); }
    protected override void Dispose(bool disposing) { if (disposing) _connection.Dispose(); base.Dispose(disposing); }
}
