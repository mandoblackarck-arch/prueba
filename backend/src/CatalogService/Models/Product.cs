namespace CatalogService.Models;
public sealed class Product { 
    public Guid Id { get; set; } = Guid.NewGuid(); 
    public string Slug { get; set; } = string.Empty; 
    public string Name { get; set; } = string.Empty; 
    public string Description { get; set; } = string.Empty; 
    public string Category { get; set; } = string.Empty; 
    public string ImageUrl { get; set; } = string.Empty; 
    public decimal Price { get; set; } 
    public int Existencia { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}
