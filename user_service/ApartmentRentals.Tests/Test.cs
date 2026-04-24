// TestsBase.cs
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting; 
namespace ApartmentRentals.Tests;

public class TestsBase<T> : WebApplicationFactory<T> where T : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
        });
    }
}