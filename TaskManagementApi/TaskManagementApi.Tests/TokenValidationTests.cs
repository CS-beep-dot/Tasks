using System.Net;
using System.Net.Http.Headers;

namespace TaskManagementApi.Tests;

public class TokenValidationTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    public TokenValidationTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<HttpResponseMessage> GetTasksWithAsync(string? authHeader)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/tasks");

        if (authHeader != null)
            request.Headers.TryAddWithoutValidation("Authorization", authHeader);

        return await _client.SendAsync(request);
    }

    [Fact]
    public async Task NoToken_Returns401()
    {
        var response = await GetTasksWithAsync(null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ExpiredToken_Returns401()
    {
        var token = TestTokens.Create(_factory, 1, "Manager", expires: DateTime.UtcNow.AddMinutes(-5));

        var response = await GetTasksWithAsync($"Bearer {token}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task MalformedToken_Returns401()
    {
        var response = await GetTasksWithAsync("Bearer abc");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ForgedToken_Returns401()
    {
        var token = TestTokens.Create(_factory, 1, "Manager",
            signingKey:"this-is-a-completely-different-fake-key-1234567890");

        var response = await GetTasksWithAsync($"Bearer {token}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WrongIssuer_Returns401()
    {
        var token = TestTokens.Create(_factory, 1, "Manager", issuer: "WrongIssuer");

        var response = await GetTasksWithAsync($"Bearer {token}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WrongAudience_Returns401()
    {
        var token = TestTokens.Create(_factory, 1, "Manager", audience: "WrongAudience");

        var response = await GetTasksWithAsync($"Bearer {token}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ValidToken_Returns200()
    {
        var token = TestTokens.Create(_factory, 1, "Manager");

        var response = await GetTasksWithAsync($"Bearer {token}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


}
