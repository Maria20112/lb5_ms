using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ApartmentRentals.KafkaProducer
{
    public interface IKafkaProducer : IDisposable
    {
        Task SendAsync(string message);
    }

    public class KafkaProducer : IKafkaProducer
    {
        private readonly ILogger<KafkaProducer> _logger;
        private readonly IProducer<string, string> _producer;
        private string _topic;
        private bool isDisposed;
        public KafkaProducer(ILogger<KafkaProducer> logger, IOptions<KafkaSettings> settings) 
        { 
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = settings.Value.BootstrapServers
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _topic = settings.Value.ProducerTopic;

        }

        public async Task SendAsync(string message)
        {
            try
            {
                await _producer.ProduceAsync(_topic, new Message<string, string> { Key = "uniq1", Value = message });
                _logger.LogInformation($"Sending {message} to {_topic}");
            } 
            catch(ProduceException<Null,string> ex)
            {
                _logger.LogError(ex, "Unable to send message. Mesage: {message}.", message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                _producer?.Dispose();
            }
            isDisposed = true;
        }
    }
}