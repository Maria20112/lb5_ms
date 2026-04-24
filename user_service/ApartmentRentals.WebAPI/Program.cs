using ApartmentRentals.Data.Models;
using ApartmentRentals.Models;
using ApartmentRentals.WebAPI.Services;
using ApartmentRentals.WebAPI.Services.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Prometheus;
using StackExchange.Redis;
using kafka.KafkaProducer;
using ApartmentRentals.KafkaConsumer;
using Confluent.Kafka;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ApartmentRentals.Tests")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Настраиваем авто маппер
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

// Не отрезай Async в названии
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddOptions<KafkaSettings>()
    .Bind(builder.Configuration.GetSection("KafkaProducer"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(sp =>
{
    var kafkaProducerSettings = sp.GetRequiredService<IOptions<KafkaSettings>>().Value;
    var config = new ProducerConfig
    {
        BootstrapServers = kafkaProducerSettings.BootstrapServers,
        Acks = kafkaProducerSettings.Acks,
        EnableIdempotence = kafkaProducerSettings.EnableIdempotence,
    };

    var producerBuilder = new ProducerBuilder<string, string>(config);
    producerBuilder.SetErrorHandler((_, error) =>
    {
        var logger = sp.GetRequiredService<ILogger<IProducer<string, string>>>();
        logger.LogError("Kafka producer error: {Error}", error.Reason);
    });

    return producerBuilder.Build();
});
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();
builder.Services.AddSingleton<KafkaProducer>();

builder.Services.AddOptions<KafkaSettings>()
    .Bind(builder.Configuration.GetSection("KafkaConsumer"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddHostedService<KafkaConsumer>();

// === Регистрируем бд ===

builder.Services.Configure<MongoDbContext>(
    builder.Configuration.GetSection("UsersDatabase"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    return new MongoClient(sp.GetRequiredService<IOptions<MongoDbContext>>().Value.ConnectionString);
});

// === регистрируем кэш ===
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["RedisCache:Configuration"];
    options.InstanceName = builder.Configuration["RedisCache:InstanceName"];
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration["RedisCache:Configuration"]));

// === Регистрация репозиториев с данными ===
/*builder.Services.AddScoped<MongoDbRepository<Landlord>>();
builder.Services.AddScoped<IRepository<Landlord>>(provider =>
{
    var r = provider.GetRequiredService<MongoDbRepository<Landlord>>();
    var c = provider.GetRequiredService<IDistributedCache>();
    return new CachedRepository<Landlord>(r, c);
});

builder.Services.AddScoped<MongoDbRepository<RentalContract>>();
builder.Services.AddScoped<IRepository<RentalContract>>(provider =>
{
    var r = provider.GetRequiredService<MongoDbRepository<RentalContract>>();
    var c = provider.GetRequiredService<IDistributedCache>();
    return new CachedRepository<RentalContract>(r, c);
});

builder.Services.AddScoped<MongoDbRepository<Space>>();
builder.Services.AddScoped<IRepository<Space>>(provider =>
{
    var r = provider.GetRequiredService<MongoDbRepository<Space>>();
    var c = provider.GetRequiredService<IDistributedCache>();
    return new CachedRepository<Space>(r, c);
});

builder.Services.AddScoped<MongoDbRepository<Tenant>>();
builder.Services.AddScoped<IRepository<Tenant>>(provider =>
{
    var r = provider.GetRequiredService<MongoDbRepository<Tenant>>();
    var c = provider.GetRequiredService<IDistributedCache>();
    return new CachedRepository<Tenant>(r, c);
});
*/
// =========

builder.Services.AddMetrics();
builder.Services.AddHostedService<MetricsUpdateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHttpMetrics();

app.MapMetrics();

app.MapControllers();

app.Run();

public partial class Program { }



