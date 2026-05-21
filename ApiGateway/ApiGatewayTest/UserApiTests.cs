using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace ApiGatewayTest;

public class UserApiTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly string _gatewayUrl;
    private string? _validToken;

    public UserApiTests()
    {
        _gatewayUrl = "http://api_gateway:8080";

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_gatewayUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
    }

    public async Task InitializeAsync()
    {
        var testEmail = $"test_user_{Guid.NewGuid():N}@test.com";
        var testPassword = "TestPass123!";

        await AuthHelper.RegisterAsync(_gatewayUrl, testEmail, testPassword);
        _validToken = await AuthHelper.GetValidTokenAsync(_gatewayUrl, testEmail, testPassword);
        _validToken.Should().NotBeNull();
    }

    public Task DisposeAsync()
    {
        _httpClient.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetUsers_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _httpClient.GetAsync("/api/User");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsers_WithFakeToken_ReturnsUnauthorized()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/User");
        AuthHelper.AddBearerToken(request, AuthHelper.GetFakeToken());

        var response = await _httpClient.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task User_FullCrudCycle_WithValidToken()
    {
        var userId = Guid.NewGuid().ToString();
        var userData = new
        {
            Id = userId,
            Email = $"user_{Guid.NewGuid():N}@test.com",
            PasswordHash = "hashed_password_here",
            Role = "Tenant"
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/User")
        {
            Content = JsonContent.Create(userData)
        };
        AuthHelper.AddBearerToken(createRequest, _validToken!);

        var createResponse = await _httpClient.SendAsync(createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var listRequest = new HttpRequestMessage(HttpMethod.Get, "/api/User");
        AuthHelper.AddBearerToken(listRequest, _validToken!);

        var listResponse = await _httpClient.SendAsync(listRequest);
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateData = new { Id = userId, Email = "updated@test.com" };
        var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/User/{userId}")
        {
            Content = JsonContent.Create(updateData)
        };
        AuthHelper.AddBearerToken(updateRequest, _validToken!);

        var updateResponse = await _httpClient.SendAsync(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/User/{userId}");
        AuthHelper.AddBearerToken(deleteRequest, _validToken!);

        var deleteResponse = await _httpClient.SendAsync(deleteRequest);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}