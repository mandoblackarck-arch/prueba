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
public sealed class AuthController(IdentityDbContext db, IConfiguration config, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        logger.LogInformation("Registration attempt received. TraceId: {TraceId}", HttpContext.TraceIdentifier);
        var email = request.Email.Trim().ToLowerInvariant();
        if (await db.Users.AnyAsync(x => x.Email == email, ct))
        {
            logger.LogWarning("Registration rejected because the email is already registered. TraceId: {TraceId}", HttpContext.TraceIdentifier);
            return Conflict(new { message = "El correo ya está registrado." });
        }

        var salt = RandomNumberGenerator.GetBytes(16);
        var user = new AppUser { Name = request.Name.Trim(), Email = email, PasswordSalt = salt, PasswordHash = Rfc2898DeriveBytes.Pbkdf2(request.Password, salt, 310_000, HashAlgorithmName.SHA512, 32) };
        try
        {
            db.Users.Add(user);
            await db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Database error while registering a user. TraceId: {TraceId}", HttpContext.TraceIdentifier);
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: "No fue posible registrar el usuario. Consulte el TraceId en los logs.");
        }

        logger.LogInformation("User registration completed. UserId: {UserId}; TraceId: {TraceId}", user.Id, HttpContext.TraceIdentifier);
        return Created(string.Empty, CreateResponse(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        logger.LogInformation("Login attempt received. TraceId: {TraceId}", HttpContext.TraceIdentifier);
        var user = await db.Users.SingleOrDefaultAsync(x => x.Email == request.Email.Trim().ToLowerInvariant(), ct);
        if (user is null || !CryptographicOperations.FixedTimeEquals(user.PasswordHash, Rfc2898DeriveBytes.Pbkdf2(request.Password, user.PasswordSalt, 310_000, HashAlgorithmName.SHA512, 32)))
        {
            logger.LogWarning("Login rejected due to invalid credentials. TraceId: {TraceId}", HttpContext.TraceIdentifier);
            return Unauthorized(new { message = "Credenciales inválidas." });
        }

        logger.LogInformation("Login completed. UserId: {UserId}; TraceId: {TraceId}", user.Id, HttpContext.TraceIdentifier);
        return Ok(CreateResponse(user));
    }

    private AuthResponse CreateResponse(AppUser user)
    {
        var expires = DateTimeOffset.UtcNow.AddMinutes(30);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
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
