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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using GenericServices.Core.Abstracts;

namespace Customers.Kafka;

public class KafkaConsumer<T, ID> : BackgroundService, IHostedService where T : AggregateEntity<ID>
{
    private readonly IConfiguration _configuration;
    private readonly string _topicName;
    private readonly ServiceProvider serviceProvider;
    private readonly ILogger<T> _logger;
    private readonly IMessageHandler<T> _messageHandler;
    private IConsumer<Ignore, string> _consumer;

    public KafkaConsumer(IConfiguration configuration, ILogger<T> logger, string topicName, ServiceProvider serviceProvider) : base()
    {

        _logger = logger;
        this._configuration = configuration;
        _topicName = topicName;
        this.serviceProvider = serviceProvider;

    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ConsumerConfig config = new ConsumerConfig();
        _configuration.GetSection("Kafka:ConsumerSettings").Bind(config);
        
        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        new Thread(() => StartConsumerLoop(stoppingToken)).Start();
        return Task.CompletedTask;
    }

    private void StartConsumerLoop(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Consumer for {_topicName} started.....");
        _consumer.Subscribe(_topicName);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumerData = _consumer.Consume();
                if (consumerData == null)
                    return;

                var dataWrapper = JsonSerializer.Deserialize<MessagingDataWrapper<T>>(consumerData.Message.Value);
                if (dataWrapper != null)
                {
                    _logger.LogInformation($"Message received : {dataWrapper}");
                    using (IServiceScope scope = serviceProvider.CreateScope())
                    {
                        IMessageHandler<T> handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<T>>();
                        if (handler != null)
                        {
                            _logger.LogInformation($"Message process starting.... : {dataWrapper}");
                            Task task = Task.Run(() => handler.HandleMessage(dataWrapper.Entity, dataWrapper.EventType));
                            Task.WaitAll(task);
                            _logger.LogInformation($"Message processed.... : {dataWrapper}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    public override void Dispose()
    {
       _consumer.Close();
       _consumer.Dispose();
       base.Dispose();
    }
}
