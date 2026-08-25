using Microsoft.EntityFrameworkCore;
using server.Data;
using server.DTOs;
using server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Client", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<AuthService>();

var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("JWT secret is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                context.NoResult();

                var (error, message) = context.Exception switch
                {
                    SecurityTokenExpiredException =>
                        ("token_expired", "The token has expired."),
                    SecurityTokenInvalidSignatureException =>
                        ("invalid_token", "The token signature is invalid."),
                    _ =>
                        ("invalid_token", "The token is invalid.")
                };

                context.HttpContext.Items["AuthError"] = (error, message);

                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();

                if (context.HttpContext.Items.TryGetValue("AuthError", out var value) &&
                    value is ValueTuple<string, string> authError)
                {
                    await WriteAuthErrorAsync(
                        context.Response,
                        StatusCodes.Status401Unauthorized,
                        authError.Item1,
                        authError.Item2);

                    return;
                }

                await WriteAuthErrorAsync(
                    context.Response,
                    StatusCodes.Status401Unauthorized,
                    "missing_token",
                    "No authentication token was provided.");
            },
            OnForbidden = context =>
            {
                return WriteAuthErrorAsync(context.Response, StatusCodes.Status403Forbidden, "forbidden", "You do not have permission to access this resource.");
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireRole("Manager"));

    options.AddPolicy("TechnicianOnly", policy =>
        policy.RequireRole("Technician"));

    options.AddPolicy("IncidentAccess", policy =>
        policy.RequireRole("Manager", "Technician"));
});

// Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Seed database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("Client");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast")
.RequireAuthorization();

app.Run();

static async Task WriteAuthErrorAsync(HttpResponse response, int statusCode, string error, string message)
{
    response.StatusCode = statusCode;
    response.ContentType = "application/json";

    var body = new ErrorResponse { Error = error, Message = message };
    var options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    await response.WriteAsync(JsonSerializer.Serialize(body, options));
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
