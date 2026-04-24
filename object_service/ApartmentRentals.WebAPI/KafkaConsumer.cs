using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ApartmentRentals.KafkaConsumer {
    public class KafkaConsumer : BackgroundService
    {
        private readonly KafkaSettings _settings;
        private readonly string _topic;
        public KafkaConsumer(IOptions<KafkaSettings> settings) 
        { 
            _settings = settings.Value;
            _topic = _settings.UserServiceTopic;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(async () => {
                var config = new ConsumerConfig
                {
                    BootstrapServers = _settings.BootstrapServers
                    //GroupId = _settings.ConsumptionGroupId,
                    //AutoOffsetReset = AutoOffsetReset.Earliest,
                    // EnableAutoCommit = true,
                    // AutoCommitIntervalMs = 5000
                };

                using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
                {

                    consumer.Subscribe(_topic);

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var consumeResult = consumer.Consume(cancellationToken);
                        if (processMessageAsync != null)
                        {
                            await processMessageAsync(consumeResult.Message?.Value);
                        }
                        // consumer.Commit(); 
                    }
                    consumer.Close();
                }
            }, cancellationToken);
        }

        public Task processMessageAsync(string str)
        {
            return Task.CompletedTask;
        }
    }
}
