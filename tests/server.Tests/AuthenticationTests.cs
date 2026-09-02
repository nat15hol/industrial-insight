using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace server.Tests;

public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string TestJwtSecret =
        "test-secret-key-that-is-long-enough-for-hs256-signing";

    private const string TestIssuer = "IndustrialInsight";
    private const string TestAudience = "IndustrialInsightClient";

    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient()
    {
        return _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Secret"] = TestJwtSecret,
                    ["Jwt:Issuer"] = TestIssuer,
                    ["Jwt:Audience"] = TestAudience,
                    ["Jwt:ExpirationMinutes"] = "60"
                });
            });

            builder.ConfigureTestServices(services =>
            {
                services.PostConfigure<JwtBearerOptions>(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        var key = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(TestJwtSecret));

                        options.TokenValidationParameters.IssuerSigningKey = key;
                        options.TokenValidationParameters.ValidIssuer = TestIssuer;
                        options.TokenValidationParameters.ValidAudience = TestAudience;
                        options.TokenValidationParameters.ValidateIssuer = true;
                        options.TokenValidationParameters.ValidateAudience = true;
                        options.TokenValidationParameters.ValidateLifetime = true;
                        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    });
            });
        }).CreateClient();
    }

    private static string CreateTestToken(string role)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(TestJwtSecret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "999"),
            new Claim(ClaimTypes.NameIdentifier, "999"),
            new Claim(JwtRegisteredClaimNames.Email, $"{role.ToLower()}@test.local"),
            new Claim(ClaimTypes.Name, $"Test {role}"),
            new Claim(ClaimTypes.Role, role)
        };

        var token = new JwtSecurityToken(
            issuer: TestIssuer,
            audience: TestAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Fact]
    public async Task Unauthenticated_request_returns_401()
    {
        var client = CreateClient();

        var response = await client.GetAsync("/api/Machine");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Technician_request_to_manager_only_endpoint_returns_403()
    {
        var client = CreateClient();

        var token = CreateTestToken("Technician");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync(
            "/api/Machine",
            new
            {
                name = "Unauthorized Test Machine",
                status = "Operational",
                runtime = 0,
                locationId = 1
            });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Manager_token_is_accepted_by_authorized_endpoint()
    {
        var client = CreateClient();

        var token = CreateTestToken("Manager");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/Machine");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.NotEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }
}