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
        private readonly KafkaConsumerSettings _consumerSettings;
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ILogger<KafkaConsumer> _logger;
        private readonly string _topic;
        //private static readonly Counter receivedObjecsCounter = Metrics.CreateCounter("client_serv_obj_received", "Received objects count");
        public KafkaConsumer(ILogger<KafkaConsumer> logger, IServiceScopeFactory scopeFactory, IOptions<KafkaConsumerSettings> settings)
        {
            _logger = logger;

            _scopeFactory = scopeFactory;
            _topic = settings.Value.UserServiceTopic;
            //_logger.LogInformation($"Initialize Consumer, {settings.Value.ProducerTopic}");
            _consumerSettings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Task.Delay(3000, cancellationToken);

            _logger.LogInformation("ExecuteAsync");

            IConsumer<string, string> consumer = null;
            try
            {
                var config = new ConsumerConfig
                {
                    BootstrapServers = _consumerSettings.BootstrapServers,
                    GroupId = _consumerSettings.ConsumerGroup,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true,
                    SessionTimeoutMs = 30000
                };
                _logger.LogInformation("Try build");

                consumer = new ConsumerBuilder<string, string>(config).Build();
                consumer.Subscribe(_topic);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(500));

                        _logger.LogInformation($"Consume {consumeResult} from {_topic}");

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
            catch (Exception e)
            {
                _logger.LogInformation($"Exception {e.Data} {e.Message}");
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
                _logger.LogInformation($"start process");
                var data = JsonSerializer.Deserialize<RegistrationObjectDTO>(message);
                _logger.LogInformation($"json : {message}   {data}");
                if (data != null)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        _logger.LogInformation("Process message ");
                        var userService = scope.ServiceProvider.GetRequiredService<UserService>();
                        await userService.registerObject(data);
                    }

                    //receivedObjecsCounter.Inc();
                }
            }
            catch (Exception e)
            {
               _logger.LogInformation($"Exception {e.Data} {e.Message}");
            }
        }
    }
}
