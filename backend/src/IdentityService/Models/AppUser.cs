namespace IdentityService.Models;

public sealed class AppUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public string Role { get; set; } = "Customer";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
