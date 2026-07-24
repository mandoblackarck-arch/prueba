using System.ComponentModel.DataAnnotations;
namespace IdentityService.Contracts;
public sealed class RegisterRequest
{
    [Required, StringLength(120, MinimumLength = 2)]
    public string Name { get; init; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required, MinLength(12), MaxLength(128)]
    public string Password { get; init; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Password { get; init; } = string.Empty;
}

public sealed record AuthResponse(string AccessToken, DateTimeOffset ExpiresAt, UserResponse User);
public sealed record UserResponse(Guid Id, string Name, string Email, string Role);
