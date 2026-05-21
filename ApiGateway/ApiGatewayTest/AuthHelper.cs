using System.Net.Http.Json;
using System.Text.Json;

namespace ApiGatewayTest;

public static class AuthHelper
{
    private static readonly HttpClient HttpClient = new();

    public record RegisterRequest(string Email, string Pass);
    public record LoginRequest(string Email, string Pass);
    public record AuthResponse(string Token, string RefreshToken, DateTime Expires);

    public static async Task<bool> RegisterAsync(string gatewayUrl, string email, string password)
    {
        var response = await HttpClient.PostAsJsonAsync(
            $"{gatewayUrl}/Auth/register",
            new { Email = email, Pass = password });

        return response.StatusCode == System.Net.HttpStatusCode.Created
            || response.StatusCode == System.Net.HttpStatusCode.BadRequest;
    }

    public static async Task<string?> GetValidTokenAsync(string gatewayUrl, string email, string password)
    {
        var response = await HttpClient.PostAsJsonAsync(
            $"{gatewayUrl}/Auth/login",
            new { Email = email, Pass = password });

        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);

        return doc.RootElement.TryGetProperty("token", out var token)
            ? token.GetString()
            : null;
    }

    public static void AddBearerToken(HttpRequestMessage request, string token)
    {
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public static string GetFakeToken() =>
        "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJmYWtlIiwiZXhwIjoxNzAwMDAwMDAwfQ.invalid_signature_here";
}