using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;
using System.Text.Json;

namespace ApiGatewayTest;

public class TenantApiTests : IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly string _gatewayUrl;
    private readonly string _testEmail;
    private readonly string _testPassword;
    private string? _validToken;

    public TenantApiTests()
    {
        _gatewayUrl = "http://api_gateway:8080";

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_gatewayUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };

        _testEmail = $"test_tenant_{Guid.NewGuid():N}@test.com";
        _testPassword = "TestPass123!";
    }

    public async Task InitializeAsync()
    {
        await AuthHelper.RegisterAsync(_gatewayUrl, _testEmail, _testPassword);

        _validToken = await AuthHelper.GetValidTokenAsync(_gatewayUrl, _testEmail, _testPassword);
        _validToken.Should().NotBeNull("токен должен быть получен после успешного логина");
    }

    public Task DisposeAsync()
    {
        _httpClient.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetTenants_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _httpClient.GetAsync("/api/Tenant");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTenants_WithFakeToken_ReturnsUnauthorized()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Tenant");
        AuthHelper.AddBearerToken(request, AuthHelper.GetFakeToken());

        var response = await _httpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTenants_WithValidToken_ReturnsSuccess()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/Tenant");
        AuthHelper.AddBearerToken(request, _validToken!);

        var response = await _httpClient.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Tenant_FullCrudCycle_WithValidToken()
    {
        var tenantData = new
        {
            FirstName = "Test",
            LastName = "User",
            Email = $"tenant_{Guid.NewGuid():N}@test.com",
            Phone = "+1234567890"
        };

        var createRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Tenant")
        {
            Content = JsonContent.Create(tenantData)
        };
        AuthHelper.AddBearerToken(createRequest, _validToken!);

        var createResponse = await _httpClient.SendAsync(createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        using JsonDocument doc = JsonDocument.Parse(await createResponse.Content.ReadAsStringAsync());
        var tenantId = doc.RootElement.GetProperty("id").GetString();

        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/Tenant/{tenantId}");
        AuthHelper.AddBearerToken(getRequest, _validToken!);

        var getResponse = await _httpClient.SendAsync(getRequest);
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateData = new { Id = tenantId, FirstName = "Updated", LastName = "Name" };
        var updateRequest = new HttpRequestMessage(HttpMethod.Put, $"/api/Tenant/{tenantId}")
        {
            Content = JsonContent.Create(updateData)
        };
        AuthHelper.AddBearerToken(updateRequest, _validToken!);

        var updateResponse = await _httpClient.SendAsync(updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/api/Tenant/{tenantId}");
        AuthHelper.AddBearerToken(deleteRequest, _validToken!);

        var deleteResponse = await _httpClient.SendAsync(deleteRequest);
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var verifyRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/Tenant/{tenantId}");
        AuthHelper.AddBearerToken(verifyRequest, _validToken!);

        var verifyResponse = await _httpClient.SendAsync(verifyRequest);
        verifyResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}