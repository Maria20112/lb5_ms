using AuthService;
using AuthService.Db;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AuthOption>(
    builder.Configuration.GetSection("AuthOption"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Не отрезай Async в названии
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetValue<string>("Database:ConnectionString");
    return new MongoClient(connectionString);
});

builder.Services.Configure<MongoDbContext>(
    builder.Configuration.GetSection("Database"));

builder.Services.AddSingleton<IRepository<UserCredentials>, MongoDbRepository>();

builder.Services.AddSingleton<TokenGenerator>();

var app = builder.Build();

//app.UseAuthentication();
//app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
