using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Prometheus;
using System.Text.Json;
using System.Threading;
using static Prometheus.MetricServerMiddleware;
using ApartmentRentals.Data.DTOs;
using ApartmentRentals.UsersService;

namespace ApartmentRentals.KafkaConsumer {

    public class KafkaConsumer : BackgroundService
    {
        private readonly KafkaSettings _consumerSettings;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly string _topic;
        //private static readonly Counter receivedObjecsCounter = Metrics.CreateCounter("client_serv_obj_received", "Received objects count");
        public KafkaConsumer(IServiceScopeFactory scopeFactory, IOptions<KafkaSettings> settings)
        {
            _scopeFactory = scopeFactory;
            _topic = settings.Value.ProducerTopic;
            _consumerSettings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(3000, cancellationToken);

            IConsumer<string, string> consumer = null;
            try
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = _consumerSettings.BootstrapServers,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    SessionTimeoutMs = 30000
                };

                consumer = new ConsumerBuilder<string, string>(config).Build();
                consumer.Subscribe(_topic);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(500));

                        if (consumeResult != null && !consumeResult.IsPartitionEOF)
                        {
                            await ProcessMessageAsync(consumeResult.Message.Value);
                        }
                    }
                    catch (ConsumeException)
                    {
                        await Task.Delay(1000, cancellationToken);
                    }
                }
            }
            catch (Exception)
            { //log it
            }
            finally
            {
                consumer?.Unsubscribe();
                consumer?.Close();
                consumer?.Dispose();
            }
        }

        private async Task ProcessMessageAsync(string message)
        {
            try
            {
                var data = JsonSerializer.Deserialize<RegistrationObjectDTO>(message);
                if (data != null)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
                        await userService.registerObject(data);
                    }

                    //receivedObjecsCounter.Inc();
                }
            }
            catch (Exception)
            {
               //It'll be better to log it
            }
        }
    }
}
