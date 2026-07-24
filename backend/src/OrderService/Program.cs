using System.Text;
using CatalogService.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<OrderDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb")));
builder.Services.AddDbContext<CatalogDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer prefix, e.g. 'Bearer eyJhbGc...'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "Bearer"
            },
            []
        }
    });
});
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? []).AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new() { ValidateIssuer=true, ValidIssuer=builder.Configuration["Jwt:Issuer"], ValidateAudience=true, ValidAudience=builder.Configuration["Jwt:Audience"], ValidateIssuerSigningKey=true, IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)), ValidateLifetime=true });
builder.Services.AddAuthorization();
var app = builder.Build(); app.UseExceptionHandler("/error"); app.MapGet("/error", () => Results.Problem()).ExcludeFromDescription(); app.UseSwagger(); app.UseSwaggerUI(); app.UseHttpsRedirection(); app.UseCors(); app.Use(async (context, next) => { Console.WriteLine($"Authorization header: {context.Request.Headers.Authorization}"); await next(); }); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); using (var scope = app.Services.CreateScope()) await scope.ServiceProvider.GetRequiredService<OrderDbContext>().Database.EnsureCreatedAsync(); app.Run(); public partial class Program { }
