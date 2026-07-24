using System.ComponentModel.DataAnnotations;
namespace OrderService.Contracts;
public sealed class CreateOrderRequest
{
    [Required, MinLength(1)]
    public IReadOnlyList<OrderLineRequest> Lines { get; set; } = [];
}

public sealed class OrderLineRequest
{
    [Required]
    public string ProductId { get; set; } = string.Empty;

    [Range(1, 50)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "999999")]
    public decimal UnitPrice { get; set; }
}

public sealed record OrderResponse(string Number, string Status, decimal Total);
