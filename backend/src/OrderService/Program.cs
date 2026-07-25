using System.Text;
using System.Diagnostics;
using CatalogService.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ ";
    options.UseUtcTimestamp = true;
});
builder.Services.AddDbContext<OrderDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("OrderDb")));
builder.Services.AddDbContext<CatalogDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("CatalogDb")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Pega únicamente el JWT. Swagger agregará automáticamente el prefijo Bearer.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateLifetime = true
    };
    o.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var authLogger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("OrderService.Authentication");
            authLogger.LogWarning(context.Exception, "JWT authentication failed. TraceId: {TraceId}", context.HttpContext.TraceIdentifier);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var authLogger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("OrderService.Authentication");
            authLogger.LogWarning("JWT challenge returned 401. Error: {Error}; Description: {Description}; TraceId: {TraceId}", context.Error, context.ErrorDescription, context.HttpContext.TraceIdentifier);
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();
var app = builder.Build();
var logger = app.Logger;
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}; Method: {Method}; Path: {Path}", context.TraceIdentifier, context.Request.Method, context.Request.Path);
    await Results.Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Ocurrió un error interno. Consulte el TraceId en los logs.").ExecuteAsync(context);
}));
app.UseSwagger(); app.UseSwaggerUI(); app.UseHttpsRedirection(); app.UseCors();
app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    using (logger.BeginScope(new Dictionary<string, object?> { ["TraceId"] = context.TraceIdentifier }))
    {
        logger.LogInformation("HTTP request started: {Method} {Path}", context.Request.Method, context.Request.Path);
        try
        {
            await next();
            logger.LogInformation("HTTP request completed: {Method} {Path} responded {StatusCode} in {ElapsedMs} ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "HTTP request failed: {Method} {Path} after {ElapsedMs} ms", context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
});
app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); using (var scope = app.Services.CreateScope()) await scope.ServiceProvider.GetRequiredService<OrderDbContext>().Database.EnsureCreatedAsync(); app.Run(); public partial class Program { }
