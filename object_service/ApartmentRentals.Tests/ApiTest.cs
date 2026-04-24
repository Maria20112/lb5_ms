using System.Net;
using ApartmentRentals.Data.Models;  

using System.Text;
using System.Text.Json;

namespace ApartmentRentals.Tests;

public class ApiTests : TestsBase<Program>
{
    private readonly HttpClient _client;
    private readonly string _tenantUrl = "/api/Tenant";
    private readonly string _landlordUrl = "/api/Landlord";
    private readonly string _spaceUrl = "/api/Space";
    private readonly string _rentalContractUrl = "/api/RentalContract";
    public ApiTests()
    {
        _client = CreateClient();
    }

    // Тест 1: Добавление 100 элементов
    [Fact]
    public async Task Add100Tenants()
    {

        for (int i = 0; i < 100; i++)
        {
            var tenant = new Tenant
            {
                Name = "tenant",
                Email = "tenant@test.com",
                Phone = "+7000000000"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(tenant),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync(_tenantUrl, content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        // Проверка: все 100 записей добавлены
        var getResponse = await _client.GetAsync(_tenantUrl);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    // Тест 2: Добавление 100 000 элементов
    [Fact]
    public async Task Add100000Tenants()
    {

        for (int i = 0; i < 100000; i++)
        {
            var tenant = new Tenant
            {
                Name = "tenant",
                Email = "tenant@test.com",
                Phone = "+7000000000"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(tenant),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync(_tenantUrl, content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }

    // Тест 3: Удаление всех элементов
    [Fact]
    public async Task DeleteAll()
    {
        // Удаляем всё
        var deleteResponse = await _client.DeleteAsync(_tenantUrl);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        deleteResponse = await _client.DeleteAsync(_landlordUrl);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        deleteResponse = await _client.DeleteAsync(_spaceUrl);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        deleteResponse = await _client.DeleteAsync(_rentalContractUrl);
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        // Проверяем, что база пуста
        var getResponse = await _client.GetAsync(_tenantUrl);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

         getResponse = await _client.GetAsync(_landlordUrl);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

         getResponse = await _client.GetAsync(_spaceUrl);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

         getResponse = await _client.GetAsync(_rentalContractUrl);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

}