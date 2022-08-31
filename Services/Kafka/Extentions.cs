using Customers.Kafka;

using GenericServices.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GenericServices.Core.Abstracts;
namespace GenericServices.Services.Kafka
{
    public static class Extentions
    {
        public static void AddKafkaProducer(this IServiceCollection services)
        {
            services.AddScoped<IMessagePublisher,KafkaProducer>();
        }

        public static void AddKafkaConsumer<T, ID,THandler>(this IServiceCollection services, string topic) where T : AggregateEntity<ID> 
        {
            services.AddScoped(typeof(IMessageHandler<T>),typeof(THandler));
            services.AddHostedService(sp =>
            {
                var consumer = new KafkaConsumer<T, ID>(sp.GetRequiredService<IConfiguration>(), sp.GetRequiredService<ILogger<T>>(),topic,services.BuildServiceProvider());
                return consumer;
            });
        }

    }
}