using Confluent.Kafka;

namespace BlogApp.Infrastructure.ExternalServices.Kafka;

public class BlogCreatedConsumer : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;

    public BlogCreatedConsumer(IConfiguration configuration)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = "blog-consumer-group-v2",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("🔥 Kafka consumer started");

        _consumer.Subscribe("blog-events");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var cr = _consumer.Consume(stoppingToken);

                Console.WriteLine("🔥 RECEIVED MESSAGE:");
                Console.WriteLine($"Key: {cr.Message.Key}");
                Console.WriteLine($"Value: {cr.Message.Value}");
                Console.WriteLine("----------------------");
            }
        }
        catch (OperationCanceledException)
        {
            // shutdown
        }
        finally
        {
            _consumer.Close(); // commit offset + leave group
        }
    }


}