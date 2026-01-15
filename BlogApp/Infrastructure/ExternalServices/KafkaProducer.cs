using System.Text.Json;
using Confluent.Kafka;

namespace BlogApp.Infrastructure.ExternalServices;

public class KafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(string topic, string key, object value)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = JsonSerializer.Serialize(value)
        };

        await _producer.ProduceAsync(topic, message);
    }
}