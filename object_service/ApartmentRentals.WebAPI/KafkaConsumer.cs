using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using ApartmentRentals.Data.DTOs;

using System.Text.Json;

namespace ApartmentRentals.KafkaConsumer {

    public class KafkaConsumer : BackgroundService
    {
        private readonly KafkaSettings _consumerSettings;
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ILogger<KafkaConsumer> _logger;
        private readonly string _topic;
        //private static readonly Counter receivedObjecsCounter = Metrics.CreateCounter("client_serv_obj_received", "Received objects count");
        public KafkaConsumer(ILogger<KafkaConsumer> logger, IServiceScopeFactory scopeFactory, IOptions<KafkaSettings> settings)
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
                var data = JsonSerializer.Deserialize<ReceivedObjectDTO>(message);

                if (data == null) return;

                if (data.ObjType == ObjectType.Event)
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
                        await eventService.UpdateConfirmationAsync(data);
                    }

                }
                else
                {
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();
                        await ticketService.UpdateConfirmationAsync(data);
                    }
                }

                //receivedCounter.Inc();
            }
            catch (Exception)
            {
                //It'll be better to log it
            }
        }
    }
}
