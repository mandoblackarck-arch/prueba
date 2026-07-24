namespace CatalogService.Contracts;
public sealed record ProductResponse(string Id, string Name, decimal Price, int Existencia, string Category, string Image, string Description);
