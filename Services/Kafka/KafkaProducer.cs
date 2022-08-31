using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using GenericServices.Services.Kafka;
using GenericServices.Core.Entities;
using GenericServices.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GenericServices.Services.Kafka
{
    public class KafkaProducer : IMessagePublisher
    {
        private readonly IProducer<Null, string> producer;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public KafkaProducer(IConfiguration configuration, ILogger<string> logger)
        {
            this._configuration = configuration;
            this._logger = logger;

            this.producer = BuildProducer();
        }

        private IProducer<Null, string> BuildProducer()
        {
            ConsumerConfig config = new ConsumerConfig();
            _configuration.GetSection("Kafka:ProducerSettings").Bind(config);
            var producer = new ProducerBuilder<Null, string>(config).Build();
            return producer;
        }

        public async Task<bool> PublishAsync<T>(string topic, T entity, EventType eventType)
        {
            try
            {
                var dataWrapper = new MessagingDataWrapper<T>(entity, eventType);
                _logger.LogInformation($"Sending message to Broker: {JsonSerializer.Serialize(dataWrapper)}");
                var result = await producer.ProduceAsync(topic, new Message<Null, string>() { Value = JsonSerializer.Serialize(dataWrapper) });
                
                _logger.LogInformation($"message to Broker is sent and {result.Status}: {JsonSerializer.Serialize(dataWrapper)}");
                return result.Status == PersistenceStatus.Persisted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }


    }
}