using ApartmentRentals.Models;
using Confluent.Kafka;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "kafka:9092";
    public string ProducerTopic {get; set;} = "prod-orders";
    public string UserServiceTopic {get ; set;} = "user-service-order";

    public Acks Acks {get; set;} = Acks.All;

    public bool EnableIdempotence {get; set;} = true;
    public string ConsumerGroup { get; set; } = "aspnet-orders-service";
    
    // 🔹 Опционально: дополнительные настройки
    public int ConnectionTimeoutMs { get; set; } = 30000;
    public int RetryCount { get; set; } = 3;
}