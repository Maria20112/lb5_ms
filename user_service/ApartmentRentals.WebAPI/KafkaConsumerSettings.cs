using ApartmentRentals.Models;
using Confluent.Kafka;

public class KafkaConsumerSettings
{
    public string BootstrapServers { get; set; } = "kafka:9092";

    public string UserServiceTopic {get ; set;} = "prod-orders";

    public Acks Acks {get; set;} = Acks.All;

    public bool EnableIdempotence {get; set;} = true;
    public string ConsumerGroup { get; set; } = "aspnet-orders-service";
    
    // 🔹 Опционально: дополнительные настройки
    public int ConnectionTimeoutMs { get; set; } = 30000;
    public int RetryCount { get; set; } = 3;
}