using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IdentityService.Contracts;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
namespace IdentityService.Controllers;
[ApiController, Route("api/auth")]
public sealed class AuthController(IdentityDbContext db, IConfiguration config) : ControllerBase
{
    [HttpPost("register")] public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(x => x.Email == email, ct)) return Conflict(new { message = "El correo ya está registrado." });
        var salt = RandomNumberGenerator.GetBytes(16);
        var user = new AppUser { Name = request.Name.Trim(), Email = email, PasswordSalt = salt, PasswordHash = Rfc2898DeriveBytes.Pbkdf2(request.Password, salt, 310_000, HashAlgorithmName.SHA512, 32) };
        db.Users.Add(user); await db.SaveChangesAsync(ct); return Created(string.Empty, CreateResponse(user));
    }
    [HttpPost("login")] public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == request.Email.Trim().ToLowerInvariant(), ct);
        if (user is null || !CryptographicOperations.FixedTimeEquals(user.PasswordHash, Rfc2898DeriveBytes.Pbkdf2(request.Password, user.PasswordSalt, 310_000, HashAlgorithmName.SHA512, 32))) return Unauthorized(new { message = "Credenciales inválidas." });
        return Ok(CreateResponse(user));
    }
    private AuthResponse CreateResponse(AppUser user)
    {
        var expires = DateTimeOffset.UtcNow.AddMinutes(30); var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var token = new JwtSecurityToken(issuer: config["Jwt:Issuer"], audience: config["Jwt:Audience"], claims:
        [
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new(ClaimTypes.Name, user.Name)
        ], expires: expires.UtcDateTime, signingCredentials: new(key, SecurityAlgorithms.HmacSha256));
        return new(new JwtSecurityTokenHandler().WriteToken(token), expires, new(user.Id, user.Name, user.Email, user.Role));
    }
}
